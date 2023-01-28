using FCMicroservices.Components.EnterpriseBUS;
using FCMicroservices.Components.EnterpriseBUS.Events;
using FCMicroservices.Components.FunctionRegistries;
using FCMicroservices.Components.Functions;
using FCMicroservices.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace FCMicroservices.Tests.Components.FunctionRegistries;

[TestClass]
public class FunctionRegistryTests
{
    [TestMethod]
    public void FunctionRegistryTest()
    {
        IServiceCollection services = new ServiceCollection();
        IFunctionRegistry reg = new FunctionRegistry(services);
        reg.Init<IHandler>(FunctionRegistry.RegisterCommandQueries);
        reg.Init<IEventSubscription>(FunctionRegistry.RegisterEvents);

        reg.Info().ToJson(true).Dump();
    }

    [Command]
    public class CommandA
    {
        public int a1 { get; set; }
        public int a2 { get; set; }
    }

    [Query]
    public class QueryB
    {
        public int q1 { get; set; }
        public int q2 { get; set; }
    }

    [Event]
    public class EventA
    {
        public int e1 { get; set; }
    }


    public class ReplyA
    {
        public int reply1 { get; set; }
    }

    public class ReplyB
    {
        public int reply2 { get; set; }
    }

    public class CommandAHandler : Handler<CommandA, ReplyA>
    {
        public override ReplyA Handle(CommandA input)
        {
            Console.WriteLine("COMMAND A");
            return new ReplyA();
        }
    }

    public class QueryBHandler : Handler<QueryB, ReplyB>
    {
        public override ReplyB Handle(QueryB input)
        {
            return new ReplyB();
        }
    }

    public class EventASubscription : EventSubscription<EventA>
    {
        public override void OnEvent(EventA input)
        {
            Console.WriteLine("EVENT A");
        }
    }
}