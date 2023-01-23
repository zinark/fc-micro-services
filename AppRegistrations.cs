using fc.micro.services.Utils;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace fc.micro.services
{
    public class AppRegistrations
    {
        string API_TITLE = AssemblyUtils.API_TITLE;
        string API_VERSION = AssemblyUtils.API_VERSION;
        string API_FULL_NAME = AssemblyUtils.API_FULL_NAME;

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
            app.UseHealthChecks("/liveness", new HealthCheckOptions() { Predicate = x => x.Tags.Contains("live") });
            app.UseHealthChecks("/readiness", new HealthCheckOptions() { Predicate = x => x.Tags.Contains("ready") });
            app.UseHealthChecks("/startup", new HealthCheckOptions() { Predicate = x => x.Tags.Contains("start") });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

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
}
