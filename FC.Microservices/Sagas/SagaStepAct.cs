using FCMicroservices.Sagas.Acts;
using FCMicroservices.Sagas.Endpoints.WalletApis;
using FCMicroservices.Utils;

namespace FCMicroservices.Sagas;

public class SagaStepAct
{
    public int DelaySeconds { get; set; }
    public HttpCall HttpCall { get; set; }


    public string Execute(SagaContext ctx, BoxMessage message)
    {
        // SAGA Contexti header diye ekle buraya
        var headers = HttpCall.Headers;
        
        if (ctx.Values.Count > 0)
        foreach (var head in ctx.Values)
        {
            
            headers[head.Key] = head.Value;
        }

        var req = new Requester()
            .WithUrl(HttpCall.Url)
            .WithBody(message.Content)
            .WithHeaders(headers)
            .WithMethod(HttpCall.Method);

        (int statusCode, string response) = req.Execute();
        Console.WriteLine(HttpCall.Url + " " + statusCode) ;

        object describe = req.Describe();
        return response;
    }
}