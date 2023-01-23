using System.Diagnostics;

namespace fc.microservices.Components.Loggers
{
    public interface ITracer
    {
        (Activity, ActivitySource) Trace(string title, string key, object value);

        (Activity, ActivitySource) Trace(string title, IDictionary<string, object> values);
    }
}