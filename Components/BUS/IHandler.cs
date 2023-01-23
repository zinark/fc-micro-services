namespace fc.microservices.Components.BUS;

public interface IHandler
{
    public object Handle(object input);
}
