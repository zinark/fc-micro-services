using FCMicroservices.Components.EnterpriseBUS.Events;
using FCMicroservices.Extensions;

namespace FCMicroservices.Tests.Messages.Events;

public class HesaplamaBittiSubscription : EventSubscription<HesaplamaBitti>
{
    public override void OnEvent(HesaplamaBitti input)
    {
        Console.WriteLine("Mesaj alindi!" + input.ToJson(true));
    }
}