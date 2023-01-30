using System.Reflection;

using FCMicroservices;
using FCMicroservices.Components.EnterpriseBUS;
using FCMicroservices.Components.EnterpriseBUS.Events;
using FCMicroservices.Components.FunctionRegistries;
using FCMicroservices.Components.Functions;
using FCMicroservices.Utils;

using Microsoft.Extensions.DependencyInjection.Extensions;

Microservice
    .Create(args)
    .WithComponents(services =>
    {
    })
    .OverrideServices(services =>
    {
        var reg = new FunctionRegistry(services);
        var path = @"C:\projects\fc\FC.Microservices\Pack.01\bin\Debug\net7.0\Pack.01.dll";
        var allTypes = AssemblyUtils.SearchTypes(Assembly.LoadFile(path));
        EnterpriseBus.Init(reg);
        NatsIOEventSubscriber.Init(reg);
        services.RemoveAll<IFunctionRegistry>();
        services.AddSingleton<IFunctionRegistry>(x =>
        {
            return reg;
        });
    })
    .OverrideApp(app =>
    {
    })
    .Run();