using FCMicroservices.Components.BUS;


namespace FCMicroservices.Tests.Messages.Queries;

public class GetFirstHandler : Handler<GetFirst, GetFirstReply>
{
    public override GetFirstReply Handle(GetFirst input)
    {
        var first = input.Numbers[input.Index];
        return new GetFirstReply()
        {
            FirstValue = first
        };
    }
}
