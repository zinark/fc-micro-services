using fc.microservices.Extensions;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace fc.microservices.Components.HealthChecks;

public class DefaultStartupHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var result = HealthCheckResult.Healthy("OK");
        return result.AsTask();
    }
}
