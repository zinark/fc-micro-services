namespace FCMicroservices.Components.EnterpriseBUS;

public class CommandAttribute : MicroMessageAttribute
{
    public CommandAttribute()
    {
        MessageType = MessageTypes.Command;
    }
}