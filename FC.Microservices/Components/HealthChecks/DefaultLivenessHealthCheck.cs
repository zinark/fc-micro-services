using FCMicroservices.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FCMicroservices.Components.HealthChecks;

public class DefaultLivenessHealthCheck : IHealthCheck
{
    private const bool broken = false;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var result = HealthCheckResult.Healthy("OK");
        if (broken) result = HealthCheckResult.Unhealthy("LIVENESS HATALI!");

        return result.AsTask();
    }
}