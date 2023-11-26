using FCMicroservices.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace FCMicroservices.MicroUtils;

public class AppRegistrations
{
    private readonly string API_FULL_NAME = AssemblyUtils.API_FULL_NAME;

    private readonly string API_TITLE = AssemblyUtils.API_TITLE;
    private readonly string API_VERSION = AssemblyUtils.API_VERSION;

    public void ApplicationSwagger(WebApplication app, string domainprefix)
    {
        domainprefix = domainprefix.Replace("/", "");
        if (domainprefix.Length > 0) domainprefix = "/" + domainprefix;
        string specUrl = $"{domainprefix}/swagger/{API_VERSION}/swagger.json";
        
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.DocExpansion(DocExpansion.None);
            c.SwaggerEndpoint(specUrl, API_TITLE);
            c.InjectStylesheet("/custom-ui/theme-flattop.css");
            c.RoutePrefix = "swagger";
        });

        app.UseReDoc(x =>
        {
            x.RoutePrefix = "api-docs";
            x.SpecUrl = specUrl;
            x.DocumentTitle = API_TITLE;
        });
    }

    public void ApplicationWeb(WebApplication app)
    {
        app.UseHealthChecks("/check/liveness", new HealthCheckOptions { Predicate = x => x.Tags.Contains("live") });
        app.UseHealthChecks("/check/readiness", new HealthCheckOptions { Predicate = x => x.Tags.Contains("ready") });
        app.UseHealthChecks("/check/startup", new HealthCheckOptions { Predicate = x => x.Tags.Contains("start") });

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        app.UseDeveloperExceptionPage();
        app.UseCors(x =>
        {
            x.AllowAnyOrigin();
            x.AllowAnyHeader();
            x.AllowAnyMethod();
        });
    }

    public void ApplicationGrpc(WebApplication app)
    {
        app.MapGrpcService<GrpcMicroService>();
        app.MapGrpcReflectionService();
        app.MapGet("/grpc", () => API_FULL_NAME);
    }
}