namespace FCMicroservices.Components.EnterpriseBUS;

public class MicroMessageAttribute : Attribute
{
    public enum MessageTypes
    {
        Command,
        Query,
        Event
    }

    public MessageTypes MessageType { get; set; }
}