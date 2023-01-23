using fc.micro.services.Extensions;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace fc.micro.services.Components.HealthChecks;

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
