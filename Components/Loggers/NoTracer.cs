using System.Diagnostics;

namespace fc.microservices.Components.Loggers
{
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
}
