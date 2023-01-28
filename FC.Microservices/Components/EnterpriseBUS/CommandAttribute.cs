namespace FCMicroservices.Components.BUS;

public class CommandAttribute : MicroMessageAttribute
{
    public CommandAttribute()
    {
        MessageType = MessageTypes.Command;
    }
}