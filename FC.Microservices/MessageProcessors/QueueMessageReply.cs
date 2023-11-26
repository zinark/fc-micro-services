namespace FCMicroservices.MessageProcessors;

public class QueueMessageReply
{
    public QueueMessage Message { get; set; }
    public bool? Success { get; set; }
    public string? Exception { get; set; }
    public string Reply { get; set; }
}