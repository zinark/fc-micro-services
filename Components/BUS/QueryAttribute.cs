namespace fc.microservices.Components.BUS
{
    public class QueryAttribute : MicroMessageAttribute
    {
        public QueryAttribute()
        {
            MessageType = MessageTypes.Query;
        }
    }
}