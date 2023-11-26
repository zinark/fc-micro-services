namespace FCMicroservices.MessageProcessors;

public class QueuePolicy
{
    public float DelayMs { get; set; } = 0;

    public QueuePolicy WithDelays(float delay)
    {
        DelayMs = delay;
        return this;
    }
}