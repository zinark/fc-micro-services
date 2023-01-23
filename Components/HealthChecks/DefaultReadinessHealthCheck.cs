using FCMicroservices.Extensions;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FCMicroservices.Components.HealthChecks;

public class DefaultReadinessHealthCheck : IHealthCheck
{
    const bool broken = false;
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var result = HealthCheckResult.Healthy("OK");

        if (broken)
        {
            result = new HealthCheckResult(context.Registration.FailureStatus, "READINESS HATALI!");
        }

        return result.AsTask();
    }
}
