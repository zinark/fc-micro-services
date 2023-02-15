using FCMicroservices.Components.EnterpriseBUS;

namespace FooService.Features.Users;

[MicroMessage]
public class SearchUsersReply
{
    public List<string> FullNames { get; set; } = new List<string>();
}