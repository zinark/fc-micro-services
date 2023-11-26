using FCMicroservices.Extensions;
using Microsoft.Extensions.Logging;

namespace FCMicroservices.Components.EnterpriseBUS.Events;

public class NoEventPublisher : IEventPublisher
{
    public void Publish<T>(T @event)
    {
        Console.WriteLine("WARNING! NO PUBSUB > " + @event.ToJson());
    }

    public void Publish(string eventType, string eventAsJson)
    {
        Console.WriteLine("WARNING! NO PUBSUB > " + eventType);
    }

    public string Queue(string eventPath, string eventAsJson, Dictionary<string, string> extras)
    {
        return "GUID-" + Guid.NewGuid().ToString();
    }
}