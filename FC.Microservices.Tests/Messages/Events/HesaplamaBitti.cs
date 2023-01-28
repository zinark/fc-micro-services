using FCMicroservices.Components.EnterpriseBUS.Events;

namespace FCMicroservices.Tests.Messages.Events;

[Event]
public class HesaplamaBitti
{
    public int ToplamTutar { get; set; }
}