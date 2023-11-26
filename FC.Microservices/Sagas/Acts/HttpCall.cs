namespace FCMicroservices.Sagas.Acts;

public class HttpCall
{
    public string Url { get; set; }
    public string Method { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
}