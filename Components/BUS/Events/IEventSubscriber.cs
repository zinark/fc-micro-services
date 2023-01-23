namespace fc.microservices.Components.BUS.Events
{
    public interface IEventSubscriber
    {
        void Listen(Type type);
    }
}