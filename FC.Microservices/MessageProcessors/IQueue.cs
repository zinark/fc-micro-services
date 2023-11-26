namespace FCMicroservices.MessageProcessors;

public interface IQueue
{
    void Enqueue(QueueMessage msg);
    QueueMessage? Dequeue();
    long Len();
    string Name { get; }
}