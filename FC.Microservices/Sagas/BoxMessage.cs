using FCMicroservices.Sagas.Endpoints.WalletApis;

namespace FCMicroservices.Sagas;

public class BoxMessage
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Content { get; set; }
    public SagaContext Context { get; set; }
}