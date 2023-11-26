namespace FCMicroservices.MessageProcessors;

public class QueueProcessor
{
    private readonly QueuePolicy _policy;
    private readonly Func<QueueMessage, QueueMessageReply> _handler;

    public QueueProcessor(QueuePolicy policy, Func<QueueMessage, QueueMessageReply> handler)
    {
        _policy = policy;
        _handler = handler;
    }

    public QueueMessageReply? Process(IQueue queue)
    {
        QueueMessage? msg = queue.Dequeue();
        if (msg == null) return null;
        Thread.Sleep(TimeSpan.FromMilliseconds(_policy.DelayMs * 1000));
        return _handler(msg);
    }

    public void StartListen(IQueue queue, QueueReplies queueReplies)
    {
        Console.WriteLine($"Started Listening queue {queue.Name} ...");
        Task.Run(() =>
        {
            while (true)
            {
                QueueMessageReply? reply = Process(queue);
                Thread.Sleep(100);
                if (reply == null) continue;
                queueReplies.Add(reply);
            }
        });
    }
}