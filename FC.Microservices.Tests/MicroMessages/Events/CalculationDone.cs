using FCMicroservices.Components.EnterpriseBUS.Events;

namespace FCMicroservices.Tests.Messages.Events;

[Event]
public class CalculationDone
{
    public int TotalAmount { get; set; }
}