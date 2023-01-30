using System.Text;

using FCMicroservices.Extensions;

using NATS.Client;

namespace FCMicroservices.Components.EnterpriseBUS.Events;

public class EventPublisher : IEventPublisher
{
    private static readonly ConnectionFactory _factory = new();
    private readonly string _url;

    public EventPublisher(string url)
    {
        _url = url;
    }

    public void Publish<T>(T @event)
    {
        if (@event == null) throw new ApiException("event bos olamaz");
        var data = Encoding.UTF8.GetBytes(@event.ToJson());
        var subject = @event.GetType().FullName;

        using var conn = _factory.CreateConnection(_url);
        conn.Publish(subject, data);
        conn.Drain();
        conn.Close();
    }

    public void Publish(string eventTypeFullName, string eventAsJson)
    {
        if (string.IsNullOrWhiteSpace(eventAsJson) == null) throw new ApiException("event bos olamaz");
        var data = Encoding.UTF8.GetBytes(eventAsJson);

        using var conn = _factory.CreateConnection(_url);
        conn.Publish(eventTypeFullName, data);
        conn.Drain();
        conn.Close();

    }
}