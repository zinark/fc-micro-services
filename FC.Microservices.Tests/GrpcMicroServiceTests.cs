using FCMicroservices.Components.EnterpriseBUS;
using FCMicroservices.Components.Functions;
using FCMicroservices.Components.Tracers;
using FCMicroservices.Extensions;
using FCMicroservices.Tests.Messages.Commands;
using FCMicroservices.Tests.Messages.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace FCMicroservices.Tests;

[TestClass]
public class GrpcMicroServiceTests
{
    [TestMethod]
    public void ToByteStringTest()
    {
        Assert.Fail();
    }

    [TestMethod]
    public void ToByteStringTest1()
    {
        Assert.Fail();
    }

    [TestMethod]
    public void GrpcMicroServiceTest()
    {
        Assert.Fail();
    }

    [TestMethod]
    public void ByteExecuteTest()
    {
        Assert.Fail();
    }

    [TestMethod]
    public void BatchByteExecuteTest()
    {
        Assert.Fail();
    }

    [TestMethod]
    public void BatchExecuteTest()
    {
        Assert.Fail();
    }

    [TestMethod]
    public void BatchQueryTest()
    {
        Assert.Fail();
    }

    [TestMethod]
    public void QueryTest()
    {
        var grpc = MakeGrpc();
        var sum = new GetFirst { Numbers = new List<int> { 1, 2, 3, 4 }, Index = 0 };
        var filter = new Filter
        {
            Type = sum.GetType().FullName,
            Json = sum.ToJson(true)
        };

        var reply = grpc.Query(filter, null).Result;

        Console.WriteLine(reply.ToJson(true));
        Console.WriteLine(reply.Error);
        Assert.IsTrue(reply.Success);
        Assert.AreEqual("Ticimax.Hello.Microservice.MicroMessages.Queries.GetFirstReply", reply.Type);
        var sumReply = reply.Json.ParseJson<GetFirstReply>();
        Assert.AreEqual(1, sumReply.FirstValue);
    }

    [TestMethod]
    public void ExecuteTest()
    {
        var grpc = MakeGrpc();
        var sum = new Sum { X = 3, Y = 2 };
        var cmd = new Command
        {
            Type = sum.GetType().FullName,
            Json = sum.ToJson(true)
        };

        var reply = grpc.Execute(cmd, null).Result;

        Console.WriteLine(reply.ToJson(true));
        Console.WriteLine(reply.Error);
        Assert.IsTrue(reply.Success);
        Assert.AreEqual("Ticimax.Hello.Microservice.MicroMessages.Commands.SumReply", reply.Type);
        var sumReply = reply.Json.ParseJson<SumReply>();
        Assert.AreEqual(5, sumReply.Sum);
    }

    private static GrpcMicroService? MakeGrpc()
    {
        IServiceCollection services = new ServiceCollection();
        var functionRegistry = new FunctionRegistry(services);
        EnterpriseBus.Init(functionRegistry);

        IServiceProvider provider = services
            .AddSingleton<ITracer, NoTracer>()
            .AddSingleton<EnterpriseBus>()
            .AddSingleton<GrpcMicroService>()
            .BuildServiceProvider();

        var grpc = provider.GetService<GrpcMicroService>();
        return grpc;
    }

    [TestMethod]
    public void ContractsTest()
    {
        var grpc = MakeGrpc();
        var contracts = grpc.Contracts(new None(), null).Result;
        contracts.ToJson(true).Dump();
        Assert.IsNotNull(contracts);
        Assert.IsTrue(contracts.Messages.Count() > 0);
    }
}