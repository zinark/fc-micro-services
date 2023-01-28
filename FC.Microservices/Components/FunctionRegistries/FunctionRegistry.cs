using System.Collections.Concurrent;
using FCMicroservices.Components.BUS;
using FCMicroservices.Components.BUS.Events;
using FCMicroservices.Components.EnterpriseBUS.Events;
using FCMicroservices.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace FCMicroservices.Components.FunctionRegistries;

public class FunctionRegistry : IFunctionRegistry
{
    public static readonly ConcurrentDictionary<string, (Type msg, Type reply)>
        MessageReplyTypes_IndexedWithHandlerName = new();

    public static readonly ConcurrentDictionary<string, Type> MessageTypes_IndexedWithMessageName = new();
    public static readonly ConcurrentDictionary<string, Type> HandlerTypes_IndexedWithMessageName = new();
    private readonly List<Function> _functions = new();
    private readonly IServiceCollection _services;

    public FunctionRegistry(IServiceCollection services)
    {
        if (services == null)
            throw new ApiException("Dependency Injection icin ServicesCollection gerekli! bos gonderilemez!");
        _services = services;
    }

    public IEnumerable<Function> ListFunctions => _functions;

    public void Init<T>(Func<Type, Function> fFunctionBuilder)
    {
        var types = AssemblyUtils.SearchTypes();
        var handlerTypes = types
            .Where(x => x.IsAssignableTo(typeof(T)))
            .Where(x => x.IsClass)
            .Where(x => !x.IsAbstract)
            .ToList();

        foreach (var type in handlerTypes)
        {
            var f = fFunctionBuilder(type);
            _functions.Add(f);
            MessageTypes_IndexedWithMessageName[f.MessageName] = f.MessageType;
            HandlerTypes_IndexedWithMessageName[f.MessageName] = f.HandlerType;
            MessageReplyTypes_IndexedWithHandlerName[f.HandlerName] = (f.MessageType, f.ReplyType);
            _services.AddTransient(type);
        }
    }


    public (bool success, Type handlerType) FindHandlerType(string messageName)
    {
        var exists = HandlerTypes_IndexedWithMessageName.ContainsKey(messageName);
        if (!exists) return (false, null);

        return (true, HandlerTypes_IndexedWithMessageName[messageName]);
    }

    public (bool success, Type messageType) FindMessage(string messageName)
    {
        var exists = MessageTypes_IndexedWithMessageName.ContainsKey(messageName);
        if (!exists) return (false, null);

        return (true, MessageTypes_IndexedWithMessageName[messageName]);
    }

    public object Info()
    {
        var functions = _functions.OrderBy(x => x.MessageName);
        var commands = MakeFunctions<CommandAttribute>(functions);
        var queries = MakeFunctions<QueryAttribute>(functions);
        var events = MakeFunctions<EventAttribute>(functions);

        var messageSignatures = functions.Select(x => new
        {
            Message = x.MessageName,
            MessageSample = x.MessageType != null ? Activator.CreateInstance(x.MessageType) : null,
            Handler = x.HandlerName,
            Reply = x.ReplyName,
            ReplySample = x.ReplyType != null ? Activator.CreateInstance(x.ReplyType) : null
        }).ToList();

        return new
        {
            Distribution =
                $"[C]={commands.Count()} [Q]={queries.Count()} [E]={events.Count()} / In Total {functions.Count()}",
            Registry = commands.Concat(queries).Concat(events).GroupBy(x => x.Item1)
                .Select(x => new
                {
                    Namespace = x.Key,
                    Total = x.Count(),
                    Messages = x.Select(t => t.Item2).ToList()
                }),
            MessageSignatures = messageSignatures
        };
    }

    public static Function RegisterCommandQueries(Type handlerType)
    {
        var baseType = handlerType.BaseType;
        var args = baseType.GetGenericArguments();
        if (args.Length != 2)
            throw new ApiException("Handler TMessage ve TReply icermiyor! Handler kullanildigindan emin olun!");

        var messageType = args[0];
        var replyType = args[1];

        var handlerName = handlerType.Namespace + "." + handlerType.Name;
        var replyName = replyType.Namespace + "." + replyType.Name;
        var messageName = messageType.Namespace + "." + messageType.Name;

        return new Function
        {
            MessageType = messageType,
            MessageName = messageName,
            HandlerType = handlerType,
            HandlerName = handlerName,
            ReplyType = replyType,
            ReplyName = replyName
        };
    }

    public static Function RegisterEvents(Type subscriptionType)
    {
        var baseType = subscriptionType.BaseType;
        var args = baseType.GetGenericArguments();
        if (args.Length != 1)
            throw new ApiException("Subscription TEvent icermiyor! Event kullanildigindan emin olun!");

        var eventType = args[0];

        var subscriptionName = subscriptionType.Namespace + "." + subscriptionType.Name;
        var eventName = eventType.Namespace + "." + eventType.Name;

        return new Function
        {
            MessageType = eventType,
            MessageName = eventName,
            HandlerType = subscriptionType,
            HandlerName = subscriptionName
        };
    }

    private static List<(string, string)> MakeFunctions<TAtt>(IOrderedEnumerable<Function> functions)
        where TAtt : Attribute
    {
        var prefix = "";
        if (typeof(TAtt).Name == typeof(CommandAttribute).Name) prefix = "[C] ";
        if (typeof(TAtt).Name == typeof(QueryAttribute).Name) prefix = "[Q] ";
        if (typeof(TAtt).Name == typeof(EventAttribute).Name) prefix = "[E] ";
        return functions
            .Where(x => x.MessageType.HasAttribute<TAtt>())
            .Select(x => (x.MessageType?.Namespace,
                    prefix + x.MessageType?.Namespace + "." + x.MessageType?.Name +
                    " --> " + x.HandlerType?.Name +
                    "() ==> " + x.ReplyType?.Name
                ))
            .ToList();
    }
}