using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FCMicroservices;

public class Probes
{
    public Type Liveness { get; set; }
    public Type Readiness { get; set; }
    public Type Startup { get; set; }

    public Probes SetLiveness<T>() where T : IHealthCheck
    {
        Liveness = typeof(T);
        return this;
    }

    public Probes SetReadiness<T>() where T : IHealthCheck
    {
        Readiness = typeof(T);
        return this;
    }

    public Probes SetStartup<T>() where T : IHealthCheck
    {
        Startup = typeof(T);
        return this;
    }
}