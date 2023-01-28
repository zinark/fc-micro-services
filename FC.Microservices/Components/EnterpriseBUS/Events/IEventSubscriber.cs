namespace FCMicroservices.Components.EnterpriseBUS.Events;

public interface IEventSubscriber
{
    void Listen(Type type);
}