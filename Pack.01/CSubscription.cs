// See https://aka.ms/new-console-template for more information
using FCMicroservices.Components.EnterpriseBUS.Events;
using FCMicroservices.Extensions;

public class CSubscription : EventSubscription<C>
{
    public override void OnEvent(C input)
    {
        Console.WriteLine(input.ToJson(true));
    }
}