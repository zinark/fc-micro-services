using System.Diagnostics;
using System.Text;
using FCMicroservices.Components.Configurations;
using FCMicroservices.Components.EnterpriseBUS;
using FCMicroservices.Components.EnterpriseBUS.Events;
using FCMicroservices.Components.Filters;
using FCMicroservices.Components.Functions;
using FCMicroservices.Components.TenantResolvers;
using FCMicroservices.Components.Tracers;
using FCMicroservices.Extensions;
using FCMicroservices.MicroUtils;
using FCMicroservices.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace FCMicroservices;

public class Microservice
{
    private Action<WebApplication> _overrideApplication = x => { };
    private Action<IServiceCollection> _overrideServices = x => { };

    private readonly List<Type> _subscriptions = new();
    private WebApplication _app;
    private WebApplicationBuilder _builder;
    private IConfigLoader _cfgLoader;
    private IFunctionRegistry _functionRegistry;
    private IFunctionRenderer _functionRenderer = new FunctionRenderer();
    private Action<IServiceCollection> _withComponents = x => { };
    private Action<Probes> _probeFunction = x => { };


    private string _domainPrefix = "";
    private Action<IServiceProvider, MessageTrack> _beforeHandle = (provider, track) => { };
    private Action<IServiceProvider, MessageTrack> _afterHandle = (provider, track) => { };
    private Action<WebApplicationBuilder> _overrideBuilder = builder => { };

    public Microservice Run()
    {
        // SERVICES
        var svcReg = RegisterDefaultServices();
        _overrideServices(_builder.Services);

        // PROBES
        var probes = new Probes();
        _probeFunction(probes);
        probes.ToJson(true).Dump("PROBES");
        svcReg.ServiceHealthChecks(_builder.Services, probes);


        _overrideBuilder(_builder);
        // APP
        _app = _builder.Build();


        _overrideApplication(_app);

        if (!string.IsNullOrWhiteSpace(_domainPrefix))
        {
            _app.UsePathBase(new PathString(_domainPrefix));
        }

        _app.UseAuthentication();
        _app.UseStaticFiles();
        _app.UseRouting();
        _app.UseRateLimiter();
        _app.UseAuthorization();


        var appReg = new AppRegistrations();
        appReg.ApplicationGrpc(_app);
        appReg.ApplicationWeb(_app);
        appReg.ApplicationSwagger(_app, _domainPrefix);

        _app.UseExceptionHandler("/error");
        var subscriber = _app.Services.GetService<IEventSubscriber>();

        var subscribers = new List<dynamic>();
        foreach (var sub in _subscriptions)
        {
            Debug.WriteLine("subscribing... " + sub.FullName);
            subscriber.Listen(sub);
            subscribers.Add(sub.Name);
        }

        subscribers.ToJson(true).Dump("SUBSCRIPTIONS");

        // _app.UseMiddleware<JsonExceptionMiddleware>();
        var functionRegistry = _app.Services.GetService<IFunctionRegistry>();

        _app.MapPost("/publish", async (HttpContext ctx, IEventPublisher pub) =>
        {
            using var reader = new StreamReader(ctx.Request.Body);
            string body = await reader.ReadToEndAsync();
            dynamic bodyJs = body.ParseJson<dynamic>();

            string eventType = (string)bodyJs["type"];
            string eventJson = (string)bodyJs["json"];
            pub.Publish(eventType, eventJson);

            ctx.Response.StatusCode = 200;
            await ctx.Response.WriteAsync("{}");
        });

        _app.Map("/", () => ApiInfo(functionRegistry));
        _app.Map("/f/{type}/{f}", (string f) =>
        {
            var found = _functionRegistry.FindFunction(f);
            return Results.Content(_functionRenderer.Render(found), "text/html", Encoding.UTF8);
        });

        JsonConvert
            .SerializeObject(ConfigLoader.History, Formatting.Indented)
            .Dump("ConfigLoader");

        _app.Run();
        return this;
    }

    public ServiceRegistrations RegisterDefaultServices()
    {
        var services = _builder.Services;
        services.AddSingleton<EnterpriseBus>();
        services.AddSingleton<QueueDriver>();

        services.AddTransient<INetworkUtils, DefaultNetworkUtils>();
        services.AddSingleton<IConfigLoader, ConfigLoader>();
        services.AddSingleton<ITenantResolver, HttpTenantResolver>();
        var use_pubsub = _cfgLoader.Load("use_pubsub", "no") == "yes";
        var use_tracer = _cfgLoader.Load("use_tracer", "no") == "yes";

        if (use_tracer)
            services.AddSingleton<ITracer, LogTracer>();
        else
            services.AddSingleton<ITracer, NoTracer>();

        if (use_pubsub)
        {
            var url = _cfgLoader.Load("PUBSUB_URL", "http://localhost:4222");

            services.AddSingleton<IEventPublisher, EventPublisher>(x =>
            {
                var cfgLoader = x.GetRequiredService<IConfigLoader>();
                return new EventPublisher(url);
            });

            services.AddSingleton<IEventSubscriber>(x =>
            {
                var cfgLoader = _app.Services.GetService<IConfigLoader>();
                var tracer = _app.Services.GetService<ITracer>();
                var serviceProvider = _app.Services;
                return new NatsIOEventSubscriber(serviceProvider, tracer, url);
            });
        }
        else
        {
            services.AddTransient<IEventPublisher, NoEventPublisher>();
            services.AddTransient<IEventSubscriber, NoEventSubscriber>();
        }


        _functionRegistry = new FunctionRegistry(services);
        services.AddSingleton(x => _functionRegistry);
        EnterpriseBus.Init(_functionRegistry, _beforeHandle, _afterHandle);
        NatsIOEventSubscriber.Init(_functionRegistry);

        _withComponents(_builder.Services);

        var svcReg = new ServiceRegistrations(_cfgLoader);
        svcReg.ServiceTelemetry(_builder.Services);
        svcReg.ServiceGrpc(_builder.Services);
        svcReg.ServiceSwagger(_builder.Services, _domainPrefix);
        svcReg.ServiceJson(_builder.Services);
        svcReg.ServiceWeb(_builder.Services);
        _builder.Services.AddControllers(options => { options.Filters.Add<ApiExceptionFilter>(); });
        _builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }).AddCookie(options =>
        {
            options.LoginPath = new PathString("/Account/Login/");
            options.AccessDeniedPath = new PathString("/Account/Forbidden/");
        });

        return svcReg;
    }

    public static Microservice Create(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureThreadPool();
        return new Microservice
        {
            _builder = builder,
            _cfgLoader = new ConfigLoader(builder.Configuration)
        };

        static void ConfigureThreadPool()
        {
            ThreadPool.SetMaxThreads(int.MaxValue, int.MaxValue);
            ThreadPool.SetMinThreads(100, 100);
        }
    }

    public Microservice UseDbContext<T>() where T : DbContext
    {
        var services = _builder.Services;
        var conn_str = _cfgLoader.Load("DB");

        Action<NpgsqlDbContextOptionsBuilder>? opts = x => x.CommandTimeout(60 * 3);
        // .EnableRetryOnFailure(
        //     maxRetryCount: 10,
        //     maxRetryDelay: TimeSpan.FromSeconds(10),
        //     errorCodesToAdd: null);

        services.AddDbContextFactory<T>(x => x.UseNpgsql(conn_str, opts).UseSnakeCaseNamingConvention());
        services.AddDbContext<T>(x => x.UseNpgsql(conn_str, opts).UseSnakeCaseNamingConvention());
        return this;
    }

    public Microservice WithProbes(Action<Probes> probeFunction)
    {
        _probeFunction = probeFunction;
        return this;
    }

    public Microservice WithComponents(Action<IServiceCollection> components)
    {
        _withComponents = components;
        return this;
    }

    public Microservice OverrideServices(Action<IServiceCollection> services)
    {
        _overrideServices = services;
        return this;
    }

    public Microservice OverrideApp(Action<WebApplication> app)
    {
        _overrideApplication = app;
        return this;
    }

    public Microservice OverrideBuilder(Action<WebApplicationBuilder> builder)
    {
        _overrideBuilder = builder;
        return this;
    }

    public Microservice WithSubscription<T>()
    {
        _subscriptions.Add(typeof(T));
        return this;
    }

    /// <summary>
    /// Example : /app 
    /// </summary>
    /// <param name="prefix"></param>
    /// <returns></returns>
    public Microservice WithDomainPrefix(string prefix)
    {
        _domainPrefix = prefix;
        return this;
    }

    private object ApiInfo(IFunctionRegistry registry)
    {
        return new
        {
            Title = AssemblyUtils.API_TITLE,
            Version = AssemblyUtils.API_VERSION,
            Configs = ConfigLoader.History,
            Registry = registry.Info(_domainPrefix)
        };
    }

    public Microservice BeforeHandle(Action<IServiceProvider, MessageTrack> fTrack)
    {
        _beforeHandle = fTrack;
        return this;
    }

    public Microservice AfterHandle(Action<IServiceProvider, MessageTrack> fTrack)
    {
        _afterHandle = fTrack;
        return this;
    }
}