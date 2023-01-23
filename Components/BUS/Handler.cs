﻿namespace fc.microservices.Components.BUS;

public abstract class Handler<T, TReply> : IHandler
{
    public abstract TReply Handle(T input);

    //[Inject]
    //public IDbContextFactory<ShipmentDbContext> CtxFactory { get; set; }

    public object Handle(object input)
    {
        if (input == null) throw new ApiException("{0} handler'ina gonderilen input bos olamaz", new
        {
            handler = GetType().FullName
        });
        if (input.GetType() != typeof(T)) throw new ApiException("{0}, Handle edilirken beklenen input {1}, fakat {2} gonderildi!", new
        {
            handler = GetType().FullName,
            beklenen = input.GetType().Name,
            gelen = typeof(T).Name
        });
        return Handle((T)input);
    }
}
