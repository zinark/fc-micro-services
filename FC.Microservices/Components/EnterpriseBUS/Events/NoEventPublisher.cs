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
}

public class NoEventSubscriber : IEventSubscriber
{
    public void Listen(Type type)
    {
        Console.WriteLine("WARNING! NO PUBSUB > " + type.Name);
    }
}