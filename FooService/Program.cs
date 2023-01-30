using FCMicroservices;

using FooService.Features.Users;

Microservice
    .Create(args)
    .WithSubscription<NewUserCreated>()
    .Run();