namespace fc.micro.services.Components.BUS
{
    public class CommandAttribute : MicroMessageAttribute
    {
        public CommandAttribute()
        {
            MessageType = MessageTypes.Command;
        }
    }
}