using System.Diagnostics;
using FCMicroservices.Utils;

namespace FCMicroservices.Components.Tracers;

public class LogTracer : ITracer
{
    private static readonly ActivitySource source = new(AssemblyUtils.API_FULL_NAME);

    // public Activity? Trace(string title, string key, object value, bool error = false)
    // {
    //     using var activity = source.StartActivity(title);
    //     activity?.SetTag(key, value);
    //     if (error) activity?.SetStatus(ActivityStatusCode.Error);
    //     return activity;
    // }
    //
    // public Activity? Trace(string title, IDictionary<string, object> values, bool error = false)
    // {
    //     var activity = source.StartActivity(title);
    //
    //     foreach (var key in values.Keys)
    //     {
    //         var value = values[key];
    //         activity?.SetTag(key, value);
    //     }
    //
    //     if (error) activity?.SetStatus(ActivityStatusCode.Error);
    //
    //     return activity;
    // }
    public (Activity, ActivitySource) Trace(string title, string key, object value, bool error = false)
    {
        using var activity = source.StartActivity(title);
        activity?.SetTag(key, value);
        if (error) activity?.SetStatus(ActivityStatusCode.Error);
        return (activity, source);
    }

    public (Activity, ActivitySource) Trace(string title, IDictionary<string, object> values, bool error = false)
    {
        var activity = source.StartActivity(title);

        foreach (var key in values.Keys)
        {
            var value = values[key];
            activity?.SetTag(key, value);
        }

        if (error) activity?.SetStatus(ActivityStatusCode.Error);

        return (activity, source);
    }

    public Activity? Trace(string title, string key, object value)
    {
        var activity = source.StartActivity(title);
        activity?.SetTag(key, value);
        return activity;
    }

    public Activity? Trace(string title, IDictionary<string, object> values)
    {
        var activity = source.StartActivity(title);

        foreach (var key in values.Keys)
        {
            var value = values[key];
            activity?.SetTag(key, value);
        }

        return activity;
    }
}