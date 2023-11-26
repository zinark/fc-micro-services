using System.Diagnostics;
using FCMicroservices.Components.Functions;
using FCMicroservices.Components.Tracers;
using FCMicroservices.Extensions;
using FCMicroservices.MessageProcessors;
using FCMicroservices.Utils;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace FCMicroservices.Components.EnterpriseBUS;

public class EnterpriseBus
{
    private static IFunctionRegistry _registry;
    private readonly IServiceProvider _provider;
    private readonly ITracer _tracer;
    private readonly QueueDriver _queueDriver;
    private static Action<IServiceProvider, MessageTrack> _AfterHandle;
    private static Action<IServiceProvider, MessageTrack> _BeforeHandle;

    public EnterpriseBus(IServiceProvider provider, ITracer tracer, QueueDriver queueDriver)
    {
        _provider = provider;
        _tracer = tracer;
        _queueDriver = queueDriver;
    }

    public static void Init(IFunctionRegistry registry, Action<IServiceProvider, MessageTrack> beforeHandle,
        Action<IServiceProvider, MessageTrack> afterHandle)
    {
        _registry = registry;
        _registry.Init<IHandler>(FunctionRegistry.RegisterCommandQueries);
        _AfterHandle = afterHandle;
        _BeforeHandle = beforeHandle;
    }

    public Task<TReply> HandleAsync<T, TReply>(T msg)
    {
        return Task.FromResult(Handle<T, TReply>(msg));
    }

    public Task<object> HandleAsync<T>(T msg)
    {
        return Task.FromResult(Handle(msg));
    }

    public Task<object> HandleAsync(string msgType, string msgBody)
    {
        return Task.FromResult(Handle(msgType, msgBody));
    }

    public TReply Handle<T, TReply>(T msg, string messageId = null)
    {
        return (TReply)Handle(msg, messageId);
    }

    public object Handle<T>(T msg, string messageId = null)
    {
        object reply = null;

        MessageTrack? tmsg = null;
        if (AssemblyUtils.HasAttribute<TrackAttribute>(msg.GetType()))
        {
            tmsg = MessageTrack.Create(msg);
            tmsg.Started();
            if (!string.IsNullOrWhiteSpace(messageId)) tmsg.MessageId = messageId;
            _BeforeHandle(_provider, tmsg);
        }

        try
        {
            var extras = new Dictionary<string, string>()
            {
                { "msgTrackId", tmsg?.MessageId },
            };
            reply = Handle(typeof(T).FullName, msg.ToJson(), extras);
            tmsg?.Finished(reply);
            MessageSucceed(tmsg);
        }
        catch (Exception ex)
        {
            tmsg?.Failed(ex);
            MessageFailed(tmsg);
            throw;
        }

        return reply;
    }

    private void MessageSucceed(MessageTrack? tmsg)
    {
        if (tmsg == null) return;
        _AfterHandle(_provider, tmsg);
    }

    private void MessageFailed(MessageTrack? tmsg)
    {
        if (tmsg == null) return;
        _AfterHandle(_provider, tmsg);
    }

    public QueueMessage Queue(QueuedCommand cmd)
    {
        // string queueName, QueuePolicy queuePolicy;
        var queueName = cmd.QueueName;
        var queuePolicy = cmd.QueuePolicy;
        var messageType = cmd.CommandType;
        var messageBody = cmd.Command;

        var qmsg = cmd.CreateQueueMessage();


        var (success, msgType) = _registry.FindMessage(messageType);
        if (!success)
            throw new ApiException("Verilen isimde bir tip yok! {0} {1} {2}", new
            {
                messageType,
                messageBody,
                registeredTypes = _registry.ListFunctions.Select(x => x.MessageName).ToList()
            });

        _tracer.Trace($"{messageType}.Queue()", new Dictionary<string, object>
        {
            { "queueUid", qmsg.QueueUid },
            { "queueMessage", qmsg.ToJson(true) }
        });

        _queueDriver.RegisterQueue(cmd.CommandType, this, queueName, queuePolicy);
        _queueDriver.AddToQueue(qmsg);
        return qmsg;
    }

    public object Handle(string messageType, string messageBody, Dictionary<string, string> extras)
    {
        string? msgTrackId = string.Empty;
        extras.TryGetValue("msgTrackId", out msgTrackId);

        var (success, msgType) = _registry.FindMessage(messageType);
        if (!success)
            throw new ApiException("Verilen isimde bir tip yok! {0} {1} {2}", new
            {
                messageType,
                messageBody,
                registeredTypes = _registry.ListFunctions.Select(x => x.MessageName).ToList()
            });

        using (_tracer.Trace($"{messageType}.Handle()", new Dictionary<string, object>
               {
                   { "type", messageType },
                   { "body", messageBody },
                   { "extras", extras },
                   { "ns", msgType.Namespace }
               }))
        {
            var input = JsonConvert.DeserializeObject(messageBody, msgType);
            var (handlerSuccess, handlerType) = _registry.FindHandlerType(msgType.FullName);
            var found = _provider.GetRequiredService(handlerType);
            if (found == null) throw new ApiException("{0} Bulunamadi!", new { handlerType.Name });
            var handler = (IHandler)found;
            handler.SetExtras(extras);
            var reply = handler.Handle(input);

            using (_tracer.Trace($"{messageType}.Reply", new Dictionary<string, object>
                   {
                       { "messageTrackId", msgTrackId },
                       { "type", reply.GetType().Name },
                       { "body", reply.ToJson(true) },
                       { "ns", reply.GetType().Namespace }
                   }))
            {
                return reply;
            }
        }
    }

    public static void CheckAttribute<T>(string msgType) where T : Attribute
    {
        var (success, messageType) = _registry.FindMessage(msgType);
        if (!messageType.HasAttribute<T>())
            throw new ApiException($"{msgType}, [{typeof(T).Name}] attribute'e sahip bir micro-message olmali!");
    }
}