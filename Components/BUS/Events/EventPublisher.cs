using fc.micro.services.Components.BUS;
using fc.microservices.Extensions;

using NATS.Client;

using System.Text;

namespace fc.microservices.Components.BUS.Events
{
    public class EventPublisher : IEventPublisher
    {
        string _url;
        static readonly ConnectionFactory _factory = new();

        public EventPublisher(string url)
        {
            _url = url;
        }

        public void Publish<T>(T @event)
        {
            if (@event == null) throw new ApiException("event bos olamaz");
            var data = Encoding.UTF8.GetBytes(@event.ToJson());
            var subject = @event.GetType().Name;

            using var conn = _factory.CreateConnection(_url);
            conn.Publish(subject, data);
            conn.Drain();
            conn.Close();
        }
    }
}