using FCMicroservices.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace FCMicroservices;

public class AppRegistrations
{
    private readonly string API_FULL_NAME = AssemblyUtils.API_FULL_NAME;

    private readonly string API_TITLE = AssemblyUtils.API_TITLE;
    private readonly string API_VERSION = AssemblyUtils.API_VERSION;

    public void ApplicationSwagger(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"/swagger/{API_VERSION}/swagger.json", API_TITLE);
            c.InjectStylesheet("/custom-ui/theme-flattop.css");
        });
    }

    public void ApplicationWeb(WebApplication app)
    {
        app.UseStaticFiles();
        app.UseRouting();
        app.UseHealthChecks("/liveness", new HealthCheckOptions { Predicate = x => x.Tags.Contains("live") });
        app.UseHealthChecks("/readiness", new HealthCheckOptions { Predicate = x => x.Tags.Contains("ready") });
        app.UseHealthChecks("/startup", new HealthCheckOptions { Predicate = x => x.Tags.Contains("start") });

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        app.UseDeveloperExceptionPage();
        app.UseCors();
    }

    public void ApplicationGrpc(WebApplication app)
    {
        app.MapGrpcService<GrpcMicroService>();
        app.MapGrpcReflectionService();
        app.MapGet("/grpc", () => API_FULL_NAME);
    }
}