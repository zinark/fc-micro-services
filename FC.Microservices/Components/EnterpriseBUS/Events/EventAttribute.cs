using FCMicroservices.Components.BUS;

namespace FCMicroservices.Components.EnterpriseBUS.Events;

public class EventAttribute : MicroMessageAttribute
{
    public EventAttribute()
    {
        MessageType = MessageTypes.Event;
    }
}