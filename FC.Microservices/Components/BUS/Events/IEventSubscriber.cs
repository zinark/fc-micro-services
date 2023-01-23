namespace FCMicroservices.Components.BUS.Events
{
    public interface IEventSubscriber
    {
        void Listen(Type type);
    }
}