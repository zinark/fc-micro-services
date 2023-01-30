// See https://aka.ms/new-console-template for more information
using FCMicroservices.Components.EnterpriseBUS;

public class AHandler : Handler<A, AReply>
{
    public override AReply Handle(A input)
    {
        Console.WriteLine("A");
        return new AReply();
    }
}
