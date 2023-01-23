using fc.microservices.Components.Configurations;
using fc.microservices.Components.HealthChecks;
using fc.microservices.Utils;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using System.Diagnostics;
using System.Text.Json;

namespace fc.microservices;

public class ServiceRegistrations
{
    string API_TITLE = AssemblyUtils.API_TITLE;
    string API_VERSION = AssemblyUtils.API_VERSION;
    string API_FULL_NAME = AssemblyUtils.API_FULL_NAME;
    private IConfigLoader cfgLoader;

    public ServiceRegistrations(IConfigLoader configLoader)
    {
        cfgLoader = configLoader;
    }

    public void ServiceWeb(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.Configure<KestrelServerOptions>(options =>
        {
            options.AllowSynchronousIO = true;
        });
        services.Configure<IISServerOptions>(options =>
        {
            options.AllowSynchronousIO = true;
        });
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });
        services.AddCors(o => o.AddDefaultPolicy(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
    }

    public void ServiceJson(IServiceCollection services)
    {
        services.AddMvc().AddNewtonsoftJson();
        services.AddControllers().AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            x.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
    }

    public void ServiceHealthChecks(IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddCheck<DefaultLivenessHealthCheck>("liveness", null, new string[] { "live" })
            .AddCheck<DefaultReadinessHealthCheck>("readiness", null, new string[] { "ready" })
            .AddCheck<DefaultStartupHealthCheck>("startup", null, new string[] { "start" });
    }

    public void ServiceHealthChecks(IServiceCollection services, Probes probes)
    {

        IHealthChecksBuilder? builder = services.AddHealthChecks();
        if (probes.Startup != null)
        {
            AddTypedCheck(builder, probes.Liveness, "liveness", null, new string[] { "live" });
        }
        else
        {
            builder.AddCheck<DefaultLivenessHealthCheck>("liveness", null, new string[] { "live" });
        }

        if (probes.Readiness != null)
        {
            AddTypedCheck(builder, probes.Readiness, "readiness", null, new string[] { "ready" });
        }
        else
        {
            builder.AddCheck<DefaultReadinessHealthCheck>("readiness", null, new string[] { "ready" });
        }

        if (probes.Startup != null)
        {
            AddTypedCheck(builder, probes.Startup, "startup", null, new string[] { "start" });
        }
        else
        {
            builder.AddCheck<DefaultStartupHealthCheck>("startup", null, new string[] { "start" });
        }
    }



    public IHealthChecksBuilder AddTypedCheck(IHealthChecksBuilder builder, Type type, string name, HealthStatus? failureStatus = null, IEnumerable<string>? tags = null, TimeSpan? timeout = null)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        return builder.Add(new HealthCheckRegistration(name, GetServiceOrCreateInstance, failureStatus, tags, timeout));

        IHealthCheck GetServiceOrCreateInstance(IServiceProvider serviceProvider) => (IHealthCheck)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, type);
    }

    public void ServiceSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
            c.SwaggerDoc(API_VERSION, new OpenApiInfo
            {
                Title = API_TITLE + " " + API_VERSION,
                Version = API_VERSION
            }));

        services.AddGrpcSwagger();
    }

    public void ServiceGrpc(IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.MaxReceiveMessageSize = 1024 * 1024 * 1024; //1GB
            options.MaxSendMessageSize = 1024 * 1024 * 1024; //1GB
            options.EnableDetailedErrors = true;
        });
        services.AddGrpcHttpApi();
        services.AddGrpcReflection();
    }

    public void ServiceTelemetry(IServiceCollection services)
    {
        string LOG_COLLECTOR = cfgLoader.Load("LOG_COLLECTOR", "http://localhost:14268");
        string LOG_AGENT = cfgLoader.Load("LOG_AGENT", "localhost");
        string LOG_AGENT_PORT = cfgLoader.Load("LOG_AGENT_PORT", "6831");

        services
            .AddOpenTelemetryTracing(opt => opt
            .AddSource(API_FULL_NAME)
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(API_FULL_NAME))
            //.AddConsoleExporter(options => { })
            .AddJaegerExporter(options =>
            {
                Debug.WriteLine("===================CHECK LOGGER");
                Debug.WriteLine("COLLECTOR : " + LOG_COLLECTOR);
                Debug.WriteLine("AGENT : " + LOG_AGENT);
                Debug.WriteLine("AGENT_PORT : " + LOG_AGENT_PORT);

                options.ExportProcessorType = OpenTelemetry.ExportProcessorType.Simple;
                options.Endpoint = new Uri(LOG_COLLECTOR);
                options.AgentHost = LOG_AGENT;
                options.AgentPort = int.Parse(LOG_AGENT_PORT);
                //options.BatchExportProcessorOptions = new OpenTelemetry.BatchExportProcessorOptions<System.Diagnostics.Activity>() {  }
            })
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.EnableGrpcAspNetCoreSupport = true;
                // options.Enrich: TODO: Enrich with body
            })
            .AddHttpClientInstrumentation(options =>
            {
                options.RecordException = true;
                options.SetHttpFlavor = true;
            })
            .AddSqlClientInstrumentation(options =>
            {
                options.RecordException = true;
                options.EnableConnectionLevelAttributes = true;
                options.SetDbStatementForText = true;
            })
            .AddHttpClientInstrumentation(options =>
            {
                options.SetHttpFlavor = true;
            })
            .AddGrpcClientInstrumentation(options => { })
        );
    }
}
