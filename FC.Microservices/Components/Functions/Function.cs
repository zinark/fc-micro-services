using FCMicroservices.Components.EnterpriseBUS;
using FCMicroservices.Components.EnterpriseBUS.Events;
using FCMicroservices.Utils;

namespace FCMicroservices.Components.Functions;

public class Function
{
    public string MicroMessageType
    {
        get
        {
            if (MessageType.HasAttribute<CommandAttribute>())
            {
                return "[C]";
            } 
            if (MessageType.HasAttribute<QueryAttribute>())
            {
                return "[Q]";
            } 
            if (MessageType.HasAttribute<EventAttribute>())
            {
                return "[E]";
            }

            return "[?]";
        }
    }

    public Type? MessageType { get; set; }
    public Type? HandlerType { get; set; }
    public Type? ReplyType { get; set; }
    public string? MessageName { get; set; }
    public string? HandlerName { get; set; }
    public string? ReplyName { get; set; }
    
    public string ConnectionAddress { get; set; }
    public string ConnectionPort { get; set; }
    public string Description { get; set; } = "No description defined";
}