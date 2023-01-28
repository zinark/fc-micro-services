using System.Diagnostics;

namespace FCMicroservices.Components.Tracers;

public class NoTracer : ITracer
{
    public (Activity, ActivitySource) Trace(string title, string key, object value)
    {
        return (null, null);
    }

    public (Activity, ActivitySource) Trace(string title, IDictionary<string, object> values)
    {
        return (null, null);
    }
}