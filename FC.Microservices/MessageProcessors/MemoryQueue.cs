using FCMicroservices.Extensions;

namespace FCMicroservices.MessageProcessors;

public class MemoryQueue : IQueue
{
    static readonly PriorityQueue<string, int> _q = new();

    public static PriorityQueue<string, int> ThreadSafeQueue
    {
        get
        {
            lock (_q)
            {
                return _q;
            }
        }
    }

    public MemoryQueue(string name = "default")
    {
        Name = name;
    }

    public void Enqueue(QueueMessage msg)
    {
        lock (_q)
        {
            msg.QueueName = Name;
            var value = msg.ToJson(true);
            _q.Enqueue(value, msg.Score);
        }
    }

    public QueueMessage? Dequeue()
    {
        string? item;
        if (ThreadSafeQueue.TryDequeue(out item, out _))
        {
            var msg = item.ParseJson<QueueMessage>();
            return msg;
        }

        return null;
    }

    public long Len()
    {
        return ThreadSafeQueue.Count;
    }

    public string Name { get; private set; }
}