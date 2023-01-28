using System.Collections.Concurrent;
using FCMicroservices.Components.FunctionRegistries;
using FCMicroservices.Components.Functions;
using FCMicroservices.Components.Tracers;
using FCMicroservices.Extensions;
using FCMicroservices.Utils;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace FCMicroservices.Components.EnterpriseBUS;

public class EnterpriseBus
{
    public static readonly ConcurrentDictionary<string, Type> RegistryMessages = new();
    private static IFunctionRegistry _registry;

    private readonly IServiceProvider _provider;
    private readonly ITracer _tracer;

    public EnterpriseBus(IServiceProvider provider, ITracer tracer)
    {
        _provider = provider;
        _tracer = tracer;
    }

    public static void Init(IFunctionRegistry registry)
    {
        _registry = registry;
        _registry.Init<IHandler>(FunctionRegistry.RegisterCommandQueries);
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

    public TReply Handle<T, TReply>(T msg)
    {
        return (TReply)Handle(msg);
    }

    public object Handle<T>(T msg)
    {
        var reply = Handle(typeof(T).FullName, msg.ToJson());
        return reply;
    }

    public object Handle(string messageType, string messageBody)
    {
        var (success, msgType) = _registry.FindMessage(messageType);
        if (!success)
            throw new ApiException("Verilen isimde bir tip yok! {0} {1} {2}", new
            {
                messageType,
                messageBody,
                registeredTypes = _registry.ListFunctions.Select(x => x.MessageName).ToList()
            });

        //var msgType = RegistryMessages[messageType];

        _tracer.Trace("Handle", new Dictionary<string, object>
        {
            { "ns", msgType.Namespace },
            { "type", messageType },
            { "body", messageBody }
        });

        var input = JsonConvert.DeserializeObject(messageBody, msgType);

        // Type handlerType = RegistryHandlersByMessages[msgType.FullName];

        var (handlerSuccess, handlerType) = _registry.FindHandlerType(msgType.FullName);


        var found = _provider.GetRequiredService(handlerType);
        if (found == null) throw new ApiException("{0} Bulunamadi!", new { handlerType.Name });
        var handler = (IHandler)found;

        var reply = handler.Handle(input);

        _tracer.Trace("Reply", new Dictionary<string, object>
        {
            { "ns", reply.GetType().Namespace },
            { "type", reply.GetType().Name },
            { "body", reply.ToJson(true) }
        });
        return reply;
    }

    public static void CheckAttribute<T>(string msgType) where T : Attribute
    {
        var (success, messageType) = _registry.FindMessage(msgType);
        //var type = RegistryMessages[msgType];
        if (!messageType.HasAttribute<T>())
            throw new ApiException($"{msgType}, [{typeof(T).Name}] attribute'e sahip bir micro-message olmali!");
    }
}