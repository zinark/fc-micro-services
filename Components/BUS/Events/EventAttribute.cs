namespace FCMicroservices.Components.BUS.Events
{
    public class EventAttribute : MicroMessageAttribute
    {
        public EventAttribute()
        {
            MessageType = MessageTypes.Event;
        }
    }
}