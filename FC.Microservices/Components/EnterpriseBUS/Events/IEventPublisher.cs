namespace FCMicroservices.Components.EnterpriseBUS.Events;

public interface IEventPublisher
{
    void Publish<T>(T @event);
    void Publish(string eventType, string eventAsJson);
    string Queue(string eventPath, string eventAsJson, Dictionary<string, string> extras);
}