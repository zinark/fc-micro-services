using FCMicroservices.Sagas.Endpoints.CardsApis;
using Microsoft.AspNetCore.Mvc;

namespace FCMicroservices.Sagas.Endpoints.WalletApis;

public class WalletApi
{
    private readonly SagaHost _sagaHost;

    public WalletApi(SagaHost sagaHost)
    {
        _sagaHost = sagaHost;
    }

    public BeginWithdrawReply Begin(BeginWithdraw cmd)
    {
        var ctx = new SagaContext()
            .Add("Authorization", "bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ0ZW5hbnQtY29kZSI6IldmN0ZMN2h0ZEpVaVJIMDhzNVd0TFE9PSIsInRlbmFudC1pZCI6InRhYUdkVkU3UlR6RWVTYTVyQkx6K3c9PSIsImltcGVyc29uYXRlZC1ieSI6Ik5YcTArR3lKdzlJdm1rK1o5anNvUGc9PSIsImlkZW50aXR5LWlkIjoiZUlsT2tteWhLQi81QkN1aVZmdmh4Zz09IiwiaWRlbnRpdHktcGhvbmUiOiJDUk5jTWRsVHprVmozN3gvS3YvNVNnPT0iLCJpZGVudGl0eS1lbWFpbCI6ImNOcnJRM0hrbWE4byswcXlCbVRzUStZMjBUY3FoTEIyLzV5SFladW1uTTg9IiwiaWRlbnRpdHktYXBpa2V5IjoidnlrVmtJOTRKSExTbDZLMUJWREpmdz09IiwiaWRlbnRpdHktZXhwIjoiS3ViMTl6TWlJSHlNb3ZYYnc1SXRiZz09IiwicGFydG5lci1pZGVudGl0eS1pZCI6Ikt1YjE5ek1pSUh5TW92WGJ3NUl0Ymc9PSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6InVzZXIiLCJleHAiOjIwMTUwNzI5MTF9.h3Boa-4kzq5dMF4qpnsvlbN0mxbM2kdTbbRRtsXDITw")
            .Add("tenant-host", "https://spec.vepara.com.tr")
            .Add("tenant", "spec")
            .Add("actorid", "4")
            .Add("walletid", "4");

        _sagaHost.Init(ctx);
        var boxMessage = _sagaHost.SendMessage(cmd.Withdraw);
        
        return new BeginWithdrawReply()
        {
            QueueId = boxMessage.Id
        };
    }

    public WithdrawReply Withdraw(Withdraw cmd)
    {
        var response = _sagaHost.SendMessage(new StartWithdraw());
        return null;
    }

    public CompleteWithdrawReply Complete(CompleteWithdraw cmd)
    {
        // db update
        return new CompleteWithdrawReply()
        {
        };
    }

    public IActionResult Notify(string reply)
    {
        // Webhook'taki adrese gore haber gider
        return null;
    }
}