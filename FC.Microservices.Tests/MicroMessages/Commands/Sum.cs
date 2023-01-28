using FCMicroservices.Components.EnterpriseBUS;

namespace FCMicroservices.Tests.Messages.Commands;

[Command]
public class Sum
{
    public string TenantId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}