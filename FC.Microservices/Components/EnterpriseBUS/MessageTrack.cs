using System.Diagnostics;

namespace FCMicroservices.Components.EnterpriseBUS;

public class MessageTrack
{
    public string MessageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool? Success { get; set; }
    public string ErrorMessage { get; set; }
    public string ErrorStack { get; set; }
    public long ElapsedMs { get; set; }

    public string MessageType { get; set; }
    public object Message { get; set; }
    public object Reply { get; set; }

    public static MessageTrack Create(object msg)
    {
        var track = new MessageTrack()
        {
            Message = msg,
            MessageType = msg.GetType().ToString(),
            CreatedAt = DateTime.UtcNow,
        };
        track.Started();
        return track;
    }

    private Stopwatch _sw;

    public void Started()
    {
        _sw = Stopwatch.StartNew();
        MessageId = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
    }

    public void Finished(object reply)
    {
        Reply = reply;
        ElapsedMs = _sw.ElapsedMilliseconds;
        Success = true;
    }

    public void Failed(Exception ex)
    {
        ErrorMessage = ex.Message;
        ErrorStack = ex.ToString();
        ElapsedMs = _sw.ElapsedMilliseconds;
        Success = false;
    }
}