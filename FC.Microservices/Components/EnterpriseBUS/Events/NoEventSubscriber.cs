namespace FCMicroservices.Components.EnterpriseBUS.Events;

public class NoEventSubscriber : IEventSubscriber
{
    public void Listen(Type type)
    {
        Console.WriteLine("WARNING! NO PUBSUB > " + type.Name);
    }
}