namespace FCMicroservices.Components.BUS.Events;

public abstract class EventSubscription<T> : IEventSubscription
{
    public void OnEvent(object input)
    {
        if (input == null)
            throw new ApiException("{0} eventhandler'ina gonderilen input bos olamaz", new
            {
                eventhandler = GetType().FullName
            });

        if (input.GetType() != typeof(T))
            throw new ApiException("{0}, Event Handle edilirken beklenen input {1}, fakat {2} gonderildi!", new
            {
                eventhandler = GetType().FullName,
                beklenen = input.GetType().Name,
                gelen = typeof(T).Name
            });
        OnEvent((T)input);
    }

    public abstract void OnEvent(T input);
}