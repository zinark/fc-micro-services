namespace FCMicroservices.Components.BUS.Events;

public interface IEventPublisher
{
    void Publish<T>(T @event);
}