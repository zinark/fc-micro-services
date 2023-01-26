using FCMicroservices.Components.BUS;
using FCMicroservices.Components.BUS.Events;
using FCMicroservices.Components.Configurations;
using FCMicroservices.Components.CustomerDomainResolvers;
using FCMicroservices.Components.FunctionRegistries;
using FCMicroservices.Components.Loggers;
using FCMicroservices.Components.Middlewares;
using FCMicroservices.Components.Networks;
using FCMicroservices.Extensions;
using FCMicroservices.Utils;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;


namespace FCMicroservices;

public class Microservice
{
    public Microservice Run()
    {
        // SERVICES
        DefaultInjections(_builder.Services);
        _injectFunction(_builder.Services);

        var svcReg = new ServiceRegistrations(_cfgLoader);
        svcReg.ServiceTelemetry(_builder.Services);
        svcReg.ServiceGrpc(_builder.Services);
        svcReg.ServiceSwagger(_builder.Services);
        svcReg.ServiceJson(_builder.Services);
        svcReg.ServiceWeb(_builder.Services);
        _svcFunction(_builder.Services);

        // PROBES
        var probes = new Probes();
        _probeFunction(probes);
        probes.ToJson(true).Dump("PROBES");
        svcReg.ServiceHealthChecks(_builder.Services, probes);

        // APP
        _app = _builder.Build();

        var subscriber = _app.Services.GetService<IEventSubscriber>();

        var subscribers = new List<dynamic>();
        foreach (var sub in _subscriptions)
        {
            Console.WriteLine("subscribing... " + sub.Name);
            subscriber.Listen(sub);
            subscribers.Add(sub.Name);
        }

        subscribers.ToJson(true).Dump("SUBSCRIPTIONS");

        _app.UseMiddleware<JsonExceptionMiddleware>();
        var functionRegistry = _app.Services.GetService<IFunctionRegistry>();
        _app.Map("/", () => ApiInfo(functionRegistry));
        _appFunction(_app);

        var appReg = new AppRegistrations();
        appReg.ApplicationGrpc(_app);
        appReg.ApplicationWeb(_app);
        appReg.ApplicationSwagger(_app);

        JsonConvert
            .SerializeObject(ConfigLoader.History, Formatting.Indented)
            .Dump("ConfigLoader");

        _app.Run();
        return this;

        void DefaultInjections(IServiceCollection services)
        {
            services.AddSingleton<EnterpriseBus>();
            services.AddTransient<INetTools, DefaultNetTools>();
            services.AddSingleton<IConfigLoader, ConfigLoader>();
            services.AddSingleton<ITenantResolver, HttpTenantResolver>();

            bool use_pubsub = _cfgLoader.Load("use_pubsub", "no") == "yes";
            bool use_tracer = _cfgLoader.Load("use_tracer", "no") == "yes";

            if (use_tracer)
                services.AddSingleton<ITracer, LogTracer>();
            else
                services.AddSingleton<ITracer, NoTracer>();

            if (use_pubsub)
            {
                string url = _cfgLoader.Load("PUBSUB_URL", "http://localhost:4222");

                services.AddTransient<IEventPublisher, EventPublisher>(x =>
                {
                    var cfgLoader = x.GetRequiredService<IConfigLoader>();
                    return new EventPublisher(url);
                });

                services.AddSingleton<IEventSubscriber>(x =>
                {
                    var cfgLoader = _app.Services.GetService<IConfigLoader>();
                    var tracer = _app.Services.GetService<ITracer>();
                    var serviceProvider = _app.Services;
                    return new EventSubscriber(serviceProvider, tracer, url);
                });
            }
            else
            {
                services.AddTransient<IEventPublisher, NoEventPublisher>();
                services.AddTransient<IEventSubscriber, NoEventSubscriber>();
            }


            _functionRegistry = new FunctionRegistry(services);
            services.AddSingleton(x => _functionRegistry);
            EnterpriseBus.Init(_functionRegistry);
            EventSubscriber.Init(_functionRegistry);
        }

    }

    public static Microservice Create(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureThreadPool();
        return new Microservice()
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
        string conn_str = _cfgLoader.Load("DB");
        services.AddDbContextFactory<T>(x => x.UseNpgsql(conn_str).UseSnakeCaseNamingConvention());
        services.AddDbContext<T>(x => x.UseNpgsql(conn_str).UseSnakeCaseNamingConvention());
        return this;
    }

    public Microservice WithProbes(Action<Probes> probeFunction)
    {
        _probeFunction = probeFunction;
        return this;
    }

    public Microservice WithComponents(Action<IServiceCollection> injectFunction)
    {
        _injectFunction = injectFunction;
        return this;
    }

    public Microservice OverrideServices(Action<IServiceCollection> svcFunction)
    {
        _svcFunction = svcFunction;
        return this;
    }

    public Microservice OverrideApp(Action<WebApplication> appFunction)
    {
        _appFunction = appFunction;
        return this;
    }

    public Microservice WithSubscription<T>()
    {
        _subscriptions.Add(typeof(T));
        return this;
    }

    object ApiInfo(IFunctionRegistry registry)
    {
        return new
        {
            Title = AssemblyUtils.API_TITLE,
            Version = AssemblyUtils.API_VERSION,
            Configs = ConfigLoader.History,
            Registry = registry.Info()
        };
    }

    WebApplicationBuilder _builder;
    WebApplication _app;
    IFunctionRegistry _functionRegistry;
    IConfigLoader _cfgLoader;
    List<Type> _subscriptions = new();

    Action<IServiceCollection> _svcFunction = (x) => { };
    Action<WebApplication> _appFunction = (x) => { };
    Action<IServiceCollection> _injectFunction = (x) => { };
    Action<Probes> _probeFunction = (x) => { };

}
