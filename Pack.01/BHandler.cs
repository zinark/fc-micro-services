// See https://aka.ms/new-console-template for more information
using FCMicroservices.Components.EnterpriseBUS;

public class BHandler : Handler<B, BReply>
{
    public override BReply Handle(B input)
    {
        Console.WriteLine("B");
        return new BReply();
    }
}
