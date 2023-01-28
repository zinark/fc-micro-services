using FCMicroservices.Components.BUS;

namespace FCMicroservices.Tests.Messages.Commands;

[MicroMessage]
public class SumReply
{
    public int Sum { get; set; }
    public string Tenant { get; internal set; }
}