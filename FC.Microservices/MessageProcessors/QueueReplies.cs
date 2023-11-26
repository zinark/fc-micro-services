using System.Collections.Concurrent;

namespace FCMicroservices.MessageProcessors;

public class QueueReplies
{
    public string Name { get; set; }
    static ConcurrentDictionary<string, QueueMessageReply> _replies = new();

    public QueueReplies(string repliesName)
    {
        Name = repliesName;
    }


    public void Add(QueueMessageReply queueMessageReply)
    {
        _replies[queueMessageReply.Message.QueueUid] = queueMessageReply;
    }

    public IDictionary<string, QueueMessageReply> Replies => _replies;

    public QueueMessageReply WaitForReply(string queueId)
    {
        while (true)
        {
            if (_replies.TryGetValue(queueId, out var reply))
            {
                return reply;
            }

            Thread.Sleep(100);
        }
    }

    public long Len()
    {
        return _replies.Count;
    }
}