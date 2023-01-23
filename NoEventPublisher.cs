using fc.microservices.Components.BUS.Events;
using fc.microservices.Extensions;

namespace fc.microservices
{
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
}