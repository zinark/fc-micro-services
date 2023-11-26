namespace FCMicroservices.Components.EnterpriseBUS;

public interface IHandler
{
    object Handle(object input);
    void SetExtras(Dictionary<string, string> extras);
}