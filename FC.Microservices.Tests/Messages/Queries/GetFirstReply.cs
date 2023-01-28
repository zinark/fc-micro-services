using FCMicroservices.Components.EnterpriseBUS;

namespace FCMicroservices.Tests.Messages.Queries;

[MicroMessage]
public class GetFirstReply
{
    public int FirstValue { get; set; }
}