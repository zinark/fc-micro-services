namespace FCMicroservices.Components.EnterpriseBUS;

public class QueryAttribute : MicroMessageAttribute
{
    public QueryAttribute()
    {
        MessageType = MessageTypes.Query;
    }
}