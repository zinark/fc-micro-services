namespace FCMicroservices.Sagas.Endpoints.WalletApis;

public class SagaContext
{
    private Dictionary<string, string> _saga = new Dictionary<string, string>();

    public Dictionary<string, string> Values => _saga;
    
    
    public SagaContext Add(string key, string value)
    {
        _saga[key] = value;
        return this;
    }
}