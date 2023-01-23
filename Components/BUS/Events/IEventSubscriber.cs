namespace fc.micro.services.Components.BUS.Events
{
    public interface IEventSubscriber
    {
        void Listen(Type type);
    }
}