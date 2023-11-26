using System.Text;
using FCMicroservices.Extensions;
using NATS.Client;

namespace FCMicroservices.Components.EnterpriseBUS.Events;

public class EventPublisher : IEventPublisher
{
    private static readonly ConnectionFactory _factory = new();
    private static IConnection _connection;
    private readonly string _url;

    static void Drain()
    {
        while (true)
        {
            lock (_connection)
            {
                Thread.Sleep(1000);
                if (_connection.IsClosed()) continue;
                if (_connection.IsDraining()) continue;
                
                _connection.Drain();
            }
        }
    }
    
    public EventPublisher(string url)
    {
        _url = url;
        if (_connection == null || _connection.IsClosed())
        {
            _connection = _factory.CreateConnection(_url);
        }
    }

    public void Publish<T>(T @event)
    {
        if (@event == null) throw new ApiException("event bos olamaz");
        var data = Encoding.UTF8.GetBytes(@event.ToJson());
        var subject = @event.GetType().FullName;

        _connection.Publish(subject, data);
        
    }

    public void Publish(string eventTypeFullName, string eventAsJson)
    {
        if (string.IsNullOrWhiteSpace(eventAsJson) == null) throw new ApiException("event bos olamaz");
        var data = Encoding.UTF8.GetBytes(eventAsJson);

        _connection.Publish(eventTypeFullName, data);
    }

    public string Queue(string eventPath, string eventAsJson, Dictionary<string, string> extras)
    {
        throw new NotImplementedException("Burdan kuyruga erisim saglayacagiz");
    }
}