using System.Diagnostics;

namespace FCMicroservices.Components.Tracers;

public interface ITracer
{
    [Obsolete("Activity'ye using ile girin")]
    (Activity, ActivitySource) Trace(string title, string key, object value, bool error = false);

    [Obsolete("Activity'ye using ile girin")]
    (Activity, ActivitySource) Trace(string title, IDictionary<string, object> values, bool error = false);
    
    Activity? Trace(string title, string key, object value);

    Activity? Trace(string title, IDictionary<string, object> values);

    
}