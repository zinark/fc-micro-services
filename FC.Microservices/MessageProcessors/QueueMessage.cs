using FCMicroservices.Extensions;

namespace FCMicroservices.MessageProcessors;

public class QueueMessage
{
    public string QueueUid { get; set; }
    public int Score { get; set; }
    public string QueueName { get; set; } = "undefined-queue";
    public string CommandType { get; set; }
    public string Command { get; set; }
    public Dictionary<string, string> Extras { get; set; } = new();

    public static QueueMessage Create()
    {
        return new QueueMessage();
    }

    private QueueMessage()
    {
    }

    public QueueMessage WithCommand<T>(T data)
    {
        return WithCommand(typeof(T).FullName, data.ToJson(true));
    }

    public QueueMessage WithCommand(string type, string cmd)
    {
        Command = cmd;
        QueueUid = Guid.NewGuid().ToString().Replace("-", "");
        CommandType = type;
        return this;
    }

    public QueueMessage AddExtras(string key, string value)
    {
        Extras[key] = value;
        return this;
    }

    public QueueMessage WithExtras(Dictionary<string, string> extras)
    {
        Extras = extras;
        return this;
    }

    public QueueMessage WithScore(int score)
    {
        Score = score;
        return this;
    }
}