using System.Diagnostics;

using FCMicroservices.Utils;

namespace FCMicroservices.Components.Loggers;

public class LogTracer : ITracer
{
    static readonly ActivitySource source = new ActivitySource(AssemblyUtils.API_FULL_NAME);
    public (Activity, ActivitySource) Trace(string title, string key, object value)
    {
        using var activity = source.StartActivity(title, ActivityKind.Internal);
        activity?.SetTag(key, value);
        return (activity, source);
    }

    public (Activity, ActivitySource) Trace(string title, IDictionary<string, object> values)
    {
        using var activity = source.StartActivity(title, ActivityKind.Internal);

        foreach (var key in values.Keys)
        {
            var value = values[key];
            activity?.SetTag(key, value);
        }

        return (activity, source);
    }
}
