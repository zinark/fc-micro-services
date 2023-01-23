namespace fc.micro.services.Components.FunctionRegistries
{
    public class Function
    {
        public Type? MessageType { get; set; }
        public Type? HandlerType { get; set; }
        public Type? ReplyType { get; set; }
        public string? MessageName { get; set; }
        public string? HandlerName { get; set; }
        public string? ReplyName { get; set; }
    }
}
