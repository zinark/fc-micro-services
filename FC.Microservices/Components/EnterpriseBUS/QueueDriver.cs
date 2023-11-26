using System.Collections.Concurrent;
using FCMicroservices.Extensions;
using FCMicroservices.MessageProcessors;

namespace FCMicroservices.Components.EnterpriseBUS;

public class QueueDriver
{
    public static readonly ConcurrentDictionary<string, IQueue> _Queues = new();
    public static readonly ConcurrentDictionary<string, QueueReplies> _QueueReplies = new();
    public static readonly ConcurrentDictionary<string, QueueProcessor> _QueueProcs = new();

    public void RegisterQueue(string commandType, EnterpriseBus bus, string queueName, QueuePolicy queuePolicy)
    {
        if (!_Queues.ContainsKey(queueName)) _Queues[queueName] = new MemoryQueue(queueName);
        if (!_QueueReplies.ContainsKey(queueName)) _QueueReplies[queueName] = new QueueReplies(queueName);

        var processorName = queueName + "/" + commandType;
        if (_QueueProcs.ContainsKey(processorName)) return;

        var queue = _Queues[queueName];
        var replies = _QueueReplies[queueName];

        var processor = new QueueProcessor(queuePolicy, msg =>
        {
            try
            {
                msg.Extras["queueId"] = msg.QueueUid;
                msg.Extras["msgTrackId"] = msg.QueueUid;
                var reply = bus.Handle(msg.CommandType, msg.Command, msg.Extras);
                return new QueueMessageReply()
                {
                    Success = true,
                    Message = msg,
                    Reply = reply.ToJson(true)
                };
            }
            catch (Exception e)
            {
                return new QueueMessageReply()
                {
                    Success = false,
                    Message = msg,
                    Exception = e.ToString(),
                };
            }
        });
        processor.StartListen(queue, replies);
        _QueueProcs[processorName] = processor;
    }

    public void AddToQueue(QueueMessage qmsg)
    {
        var queue = _Queues[qmsg.QueueName];
        queue.Enqueue(qmsg);
    }
}