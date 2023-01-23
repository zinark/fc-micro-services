namespace fc.micro.services.Components.BUS
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