namespace FCMicroservices.Components.EnterpriseBUS.Events;

public interface IEventPublisher
{
    void Publish<T>(T @event);
}