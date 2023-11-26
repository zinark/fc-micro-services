namespace FCMicroservices.Components.EnterpriseBUS;

public abstract class Handler<T, TReply> : IHandler
{
    private Dictionary<string, string> _extras;
    //[Inject]
    //public IDbContextFactory<ShipmentDbContext> CtxFactory { get; set; }


    protected string GetMessageTrackId()
    {
        var result = "";
        _extras.TryGetValue("msgTrackId", out result);
        return result;
    }

    protected string GetMessageId()
    {
        var result = "";
        _extras.TryGetValue("msgid", out result);
        return result;
    }

    protected IDictionary<string, string> GetExtras()
    {
        return _extras;
    }

    public void SetExtras(Dictionary<string, string> extras)
    {
        _extras = extras;
    }

    public object Handle(object input)
    {
        if (input == null)
            throw new ApiException("{0} handler'ina gonderilen input bos olamaz", new
            {
                handler = GetType().FullName
            });
        if (input.GetType() != typeof(T))
            throw new ApiException("{0}, Handle edilirken beklenen input {1}, fakat {2} gonderildi!", new
            {
                handler = GetType().FullName,
                beklenen = input.GetType().Name,
                gelen = typeof(T).Name
            });
        return Handle((T)input);
    }

    public abstract TReply Handle(T input);
}