using System.Diagnostics;

namespace FCMicroservices.Components.Tracers;

public interface ITracer
{
    (Activity, ActivitySource) Trace(string title, string key, object value);

    (Activity, ActivitySource) Trace(string title, IDictionary<string, object> values);
}