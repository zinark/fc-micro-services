using FCMicroservices.Components.EnterpriseBUS.Events;
using FCMicroservices.Extensions;

namespace FooService.Features.Users;

public class NewUserCreatedSubscription : EventSubscription<NewUserCreated>
{
    public override void OnEvent(NewUserCreated input)
    {
        Console.WriteLine("new user : " + input.ToJson());
    }
}