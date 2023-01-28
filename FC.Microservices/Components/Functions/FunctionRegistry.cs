using System.Collections.Concurrent;
using FCMicroservices.Components.EnterpriseBUS;
using FCMicroservices.Components.EnterpriseBUS.Events;
using FCMicroservices.Components.FunctionRegistries;
using FCMicroservices.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace FCMicroservices.Components.Functions;

public class FunctionRegistry : IFunctionRegistry
{
    public static readonly ConcurrentDictionary<string, (Type msg, Type reply)>
        MessageReplyTypesIndexedWithHandlerName = new();

    public static readonly ConcurrentDictionary<string, Type> MessageTypesIndexedWithMessageName = new();
    public static readonly ConcurrentDictionary<string, Type> HandlerTypesIndexedWithMessageName = new();
    private readonly List<Function> _functions = new();
    private readonly IServiceCollection _services;

    public FunctionRegistry(IServiceCollection services)
    {
        if (services == null)
            throw new ApiException("Dependency Injection icin ServicesCollection gerekli! bos gonderilemez!");
        _services = services;
    }

    public IEnumerable<Function> ListFunctions => _functions;

    public Function FindFunction(string messageName)
    {
        var found = ListFunctions
            .Where(x => x.MessageName.ToLowerInvariant() == messageName.ToLowerInvariant())
            .FirstOrDefault();

        if (found == null)
        {
            var nearest = ListFunctions
                .Where(x => x.MessageName.ToLowerInvariant().Contains(messageName.ToLowerInvariant()))
                .Select((x => x.MessageName))
                .ToList();

            throw new ApiException("{0} isimli mesaj yok. Fakat benzer olanlar {1}", new
            {
                messageName,
                Nearest = string.Join(" ", nearest)
            });
        }

        return found;
    }

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
            MessageTypesIndexedWithMessageName[f.MessageName] = f.MessageType;
            HandlerTypesIndexedWithMessageName[f.MessageName] = f.HandlerType;
            MessageReplyTypesIndexedWithHandlerName[f.HandlerName] = (f.MessageType, f.ReplyType);
            _services.AddTransient(type);
        }
    }


    public (bool success, Type handlerType) FindHandlerType(string messageName)
    {
        var exists = HandlerTypesIndexedWithMessageName.ContainsKey(messageName);
        if (!exists) return (false, null);

        return (true, HandlerTypesIndexedWithMessageName[messageName]);
    }

    public (bool success, Type messageType) FindMessage(string messageName)
    {
        var exists = MessageTypesIndexedWithMessageName.ContainsKey(messageName);
        if (!exists) return (false, null);

        return (true, MessageTypesIndexedWithMessageName[messageName]);
    }

    public object Info()
    {
        var functions = _functions.OrderBy(x => x.MessageName);
        var commands = MakeFunctions<CommandAttribute>(functions);
        var queries = MakeFunctions<QueryAttribute>(functions);
        var events = MakeFunctions<EventAttribute>(functions);

        return new
        {
            TotalFunctions = functions.Count(),
            Distribution = $"[C]={commands.Count()} [Q]={queries.Count()} [E]={events.Count()}",
            Functions = commands.Concat(queries).Concat(events)
                .GroupBy(x => x.Namespace)
                .Select(ns => new
                {
                    Namespace = ns.Key,
                    // Total = ns.Count(),
                    Messages = ns.GroupBy(x => x.MessageType).Select(x => new
                    {
                        MessageType = x.Key,
                        // Quantity = x.Count(),
                        Messages = x.Select(x=>x.Url).ToList()
                    })
                })
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
            ReplyName = replyName,
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

    class RegisteredFunctionInfo
    {
        public string Namespace { get; set; }
        public string MessageType { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
    }

    private static List<RegisteredFunctionInfo> MakeFunctions<TAtt>(IOrderedEnumerable<Function> functions)
        where TAtt : Attribute
    {
        return functions
            .Where(x => x.MessageType.HasAttribute<TAtt>())
            .Select(x => new RegisteredFunctionInfo()
            {
                Namespace = x.MessageType?.Namespace,
                Description = x.MicroMessageType + " " + x.MessageType?.Namespace + "." + x.MessageType?.Name +
                              " --> " + x.HandlerType?.Name + "() " +
                              " " + x.ReplyType?.Name,
                Url = "/f/" + x.MessageName,
                MessageType = x.MicroMessageType
            })
            .ToList();
    }
}