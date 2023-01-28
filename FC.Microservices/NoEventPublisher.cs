using FCMicroservices.Components.BUS.Events;
using FCMicroservices.Extensions;

namespace FCMicroservices;

public class NoEventPublisher : IEventPublisher
{
    public void Publish<T>(T @event)
    {
        Console.WriteLine("WARNING! NO PUBSUB > " + @event.ToJson());
    }
}

public class NoEventSubscriber : IEventSubscriber
{
    public void Listen(Type type)
    {
        Console.WriteLine("WARNING! NO PUBSUB > " + type.Name);
    }
}