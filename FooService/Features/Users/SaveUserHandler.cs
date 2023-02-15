using FCMicroservices.Components.EnterpriseBUS;
using FCMicroservices.Components.EnterpriseBUS.Events;

namespace FooService.Features.Users;

public class SaveUserHandler : Handler<SaveUser, SaveUserReply>
{
    private IEventPublisher _publisher;

    public SaveUserHandler(IEventPublisher publisher)
    {
        _publisher = publisher;
    }
    public override SaveUserReply Handle(SaveUser input)
    {
        _publisher.Publish(new NewUserCreated() { 
            UserId = input.UserId,
            FullName = input.FullName
        });
        return new SaveUserReply();
    }
}