using System.Text;
using FCMicroservices.Components.FunctionRegistries;
using FCMicroservices.Components.Tracers;
using FCMicroservices.Utils;
using NATS.Client;
using Newtonsoft.Json;

namespace FCMicroservices.Components.EnterpriseBUS.Events;

public class NatsIOEventSubscriber : IDisposable, IEventSubscriber
{
    private static IFunctionRegistry _registry;
    private static IEnumerable<Type> _registeredEvents;
    private readonly IConnection _connection;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITracer _tracer;
    private readonly string _url;

    public NatsIOEventSubscriber(IServiceProvider serviceProvider, ITracer tracer, string url)
    {
        _url = url;
        if (string.IsNullOrWhiteSpace(url))
            throw new ApiException("EventSubscriber baglanti kuracagi Queue icin url bos olamaz!");
        ConnectionFactory factory = new();
        _tracer = tracer;
        try
        {
            _connection = factory.CreateConnection(_url);
        }
        catch (Exception? ex)
        {
            throw new ApiException(
                "NATS.IO icin {0} baglanti adresine erisilemedi! Event kullanmak icin queue gerekiyor. " +
                Environment.NewLine +
                "Event calismasini istemiyorsaniz, appsettings.json dosyasinda 'use_pubsub': 'no' ekleyebilirsiniz." +
                Environment.NewLine +
                "Ya da Docker'la bir ornek olusturun 'docker run nats' veya calisan bir nats.io queue url'i verin",
                new { _url }, ex);
        }

        _serviceProvider = serviceProvider;
    }

    public void Dispose()
    {
        _connection.Close();
    }

    public void Listen(Type type)
    {
        var (success, subscriptionType) = _registry.FindHandlerType(type.FullName);
        if (!success) throw new ApiException("Uygun eventsubscription kayitlanmamis! {0}", new { type.Name });
        var subscription = (IEventSubscription)_serviceProvider.GetService(subscriptionType);

        var subject = type.Name;
        Subscribe(subject, x => { subscription.OnEvent(x); });
    }

    public static void Init(IFunctionRegistry registry)
    {
        _registry = registry;
        registry.Init<IEventSubscription>(FunctionRegistry.RegisterEvents);
        _registeredEvents = AssemblyUtils.SearchTypes()
            .Where(x => x.HasAttribute<EventAttribute>())
            .Where(x => !x.IsAbstract)
            .Where(x => x.IsClass)
            .ToList();
    }

    public void Subscribe(Type type, IEventSubscription subscription)
    {
        var subject = type.Name;
        Subscribe(subject, x => { subscription.OnEvent(x); });
    }

    public void Subscribe<T>(Action<T> onMessage)
    {
        var subject = typeof(T).Name;
        Subscribe(subject, x => { onMessage((T)x); });
    }

    public void Subscribe(string subject, Action<object> onMessage)
    {
        //var servers = _connection.DiscoveredServers;
        //_connection.DiscoveredServers.ToJson(true).Dump("Servers");
        //_connection.Stats.ToJson(true).Dump("Stats");

        var result = _connection.SubscribeAsync(subject, (sender, args) =>
        {
            var type = _registeredEvents.Where(x => x.Name.Contains(subject)).FirstOrDefault();

            var data_bytes = args.Message.Data;
            var data_string = Encoding.UTF8.GetString(data_bytes);

            var data_json = JsonConvert.DeserializeObject(data_string, type);
            // TODO : FERHAT ! typless trytoparsejson needed
            // var (success, data_json) = data_string.TryToParseJson<T>();
            //if (!success) throw new ApiException("Event olarak gelen mesaj {0} tipine cevrilemedi!", new { subject });
            try
            {
                onMessage(data_json);
                _tracer.Trace("EVENT> " + type.Namespace, new Dictionary<string, object>
                {
                    { "ns", type.Namespace },
                    { "type", type.Name },
                    { "body", data_string }
                });

                args.Message.Ack();
            }
            catch (Exception? exc)
            {
                args.Message.Nak();

                _tracer.Trace("EVENT EXCEPTION!> " + type.Namespace, new Dictionary<string, object>
                {
                    { "ns", type.Namespace },
                    { "type", type.Name },
                    { "exception", exc.ToString() }
                });

                throw new ApiException("Gelen event icin {0} islem yapilamadi!", new
                {
                    data_json
                }, exc);
            }
        });
    }
}