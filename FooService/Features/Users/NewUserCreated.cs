using FCMicroservices.Components.EnterpriseBUS.Events;

namespace FooService.Features.Users;

[Event]
public class NewUserCreated
{
    public int UserId { get; set; }
    public string? FullName { get; set; }
}