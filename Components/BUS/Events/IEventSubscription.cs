namespace fc.microservices.Components.BUS.Events;

public interface IEventSubscription
{
    public void OnEvent(object input);
}
