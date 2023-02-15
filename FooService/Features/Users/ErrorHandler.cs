using FCMicroservices.Components;
using FCMicroservices.Components.EnterpriseBUS;

namespace FooService.Features.Users;

public class ErrorHandler : Handler<Error, ErrorReply>
{
    public override ErrorReply Handle(Error input)
    {
        throw new ApiException("at {0} Error {1} detail {2}", new { x = 1, y = 2, z =3 });
    }
}