using System.Collections.Concurrent;
using FCMicroservices.Sagas.Endpoints.WalletApis;

namespace FCMicroservices.Sagas;

public class Box
{
    public string Name { get; set; }

    public ConcurrentDictionary<string, ConcurrentQueue<BoxMessage>> Messages { get; set; } = new();

    public BoxMessage AddMessage(SagaContext ctx, string message)
    {
        var target = Name;
        foreach (var i in ctx.Values) target = target.Replace("${" + i.Key + "}", i.Value);

        var boxid = $"{GetType().Name.ToLower()}-" + Guid.NewGuid();
        var msg = new BoxMessage()
        {
            Id = boxid,
            Content = message,
            CreatedAt = DateTime.Now,
            Context = ctx
        };

        if (!Messages.ContainsKey(target)) Messages[target] = new ConcurrentQueue<BoxMessage>();
        Messages[target].Enqueue(msg);
        return msg;
    }

    public BoxMessage? Dequeue(SagaContext ctx)
    {
        var target = Name;
        foreach (var i in ctx.Values) target = target.Replace("${" + i.Key + "}", i.Value);
        if (!Messages.ContainsKey(target))
        {
            return null;
        }

        if (Messages[target].TryDequeue(out var item))
        {
            return item;
        }

        return null;
    }
}