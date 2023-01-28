using FCMicroservices.Components.EnterpriseBUS.Events;
using FCMicroservices.Extensions;

namespace FCMicroservices.Tests.Messages.Events;

public class CalculationDoneSubscription : EventSubscription<CalculationDone>
{
    public override void OnEvent(CalculationDone input)
    {
        Console.WriteLine("Done!" + input.ToJson(true));
    }
}