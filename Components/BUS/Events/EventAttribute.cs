namespace fc.microservices.Components.BUS.Events
{
    public class EventAttribute : MicroMessageAttribute
    {
        public EventAttribute()
        {
            MessageType = MessageTypes.Event;
        }
    }
}