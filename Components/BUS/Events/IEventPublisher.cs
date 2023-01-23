namespace fc.micro.services.Components.BUS.Events
{
    public interface IEventPublisher
    {
        void Publish<T>(T @event);
    }
}