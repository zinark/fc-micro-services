using FCMicroservices.Components.EnterpriseBUS;

namespace FooService.Features.Users;

public class SearchUsersHandler : Handler<SearchUsers, SearchUsersReply>
{
    public override SearchUsersReply Handle(SearchUsers input)
    {
        return new SearchUsersReply()
        {
            FullNames = new List<string>()
            {
                "jack", "john", "elisa", "lora"
            }
        };
    }
}