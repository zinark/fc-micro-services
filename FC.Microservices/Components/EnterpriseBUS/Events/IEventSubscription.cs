namespace FCMicroservices.Components.EnterpriseBUS.Events;

public interface IEventSubscription
{
    public void OnEvent(object input);
}