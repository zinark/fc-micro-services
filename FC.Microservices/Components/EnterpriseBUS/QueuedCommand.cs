using FCMicroservices.Extensions;
using FCMicroservices.MessageProcessors;

namespace FCMicroservices.Components.EnterpriseBUS;

public class QueuedCommand
{
    public Dictionary<string, string> Extras { get; set; } = new Dictionary<string, string>();
    public string QueueName { get; set; }
    public QueuePolicy QueuePolicy { get; set; }

    public static QueuedCommand Create<T> (T cmd, QueuePolicy queuePolicy)
    {
        return new QueuedCommand()
        {
            CommandType = typeof(T).FullName,
            Command = cmd.ToJson(true),
            QueuePolicy = queuePolicy,
        };
    }

    public QueuedCommand WithExtras(string key, string value)
    {
        Extras[key] = value;
        return this;
    }

    public QueuedCommand WithQueueName(string queuename)
    {
        QueueName = queuename;
        return this;
    }
    
    internal QueueMessage CreateQueueMessage()
    {
        QueueMessage qmsg = QueueMessage.Create()
            .WithCommand(CommandType, Command)
            .WithScore(1)
            .WithExtras(Extras);
        qmsg.QueueName = QueueName;
        return qmsg;
    }
    
    public string CommandType { get; set; }
    public string Command { get; set; }

}