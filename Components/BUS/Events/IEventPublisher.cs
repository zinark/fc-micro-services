namespace fc.microservices.Components.BUS.Events
{
    public interface IEventPublisher
    {
        void Publish<T>(T @event);
    }
}