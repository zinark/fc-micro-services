namespace fc.micro.services.Components.BUS
{
    public class QueryAttribute : MicroMessageAttribute
    {
        public QueryAttribute()
        {
            MessageType = MessageTypes.Query;
        }
    }
}