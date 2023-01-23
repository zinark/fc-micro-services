using FCMicroservices.Components.BUS;

namespace FCMicroservices.Tests.Messages.Queries;

[MicroMessage]
public class GetFirstReply
{
    public int FirstValue { get; set; }
}
