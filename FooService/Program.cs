using FCMicroservices;
using FCMicroservices.Components.Middlewares;
using FooService.Features.Users;
using Microsoft.AspNetCore.Builder;

Microservice
    .Create(args)
    .OverrideApp(x=>x.UseMiddleware<JsonExceptionMiddleware>())
    .WithSubscription<NewUserCreated>()
    .Run();