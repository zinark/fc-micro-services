namespace FCMicroservices.Components.BUS
{
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
}