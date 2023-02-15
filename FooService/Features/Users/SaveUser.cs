using FCMicroservices.Components.EnterpriseBUS;

namespace FooService.Features.Users;

[Command]
public class SaveUser
{
    public int UserId { get; set; }
    public string? FullName { get; set; }
}