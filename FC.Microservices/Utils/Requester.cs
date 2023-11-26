using System.Net;
using System.Net.Http.Headers;
using System.Text;
using FCMicroservices.Components;
using FCMicroservices.Extensions;

namespace FCMicroservices.Utils;

public class Requester
{
    readonly int TRY_COUNT = 3;
    readonly int TRY_DELAY_IN_SECONDS = 2;
    readonly int TIMEOUT_IN_MINUTES = 1;

    string _baseUrl;
    private string _method;
    string _queryParams;
    string _body;
    private string _contentType;
    Dictionary<string, string> _headers;
    private HttpResponseMessage _responseMessage;
    private HttpRequestMessage _requestMessage;

    public Requester(int tryCount = 3, int tryDelay = 2, int timeout = 1)
    {
        TRY_COUNT = tryCount;
        TRY_DELAY_IN_SECONDS = tryDelay;
        TIMEOUT_IN_MINUTES = timeout;
    }

    public (int statusCode, string response) Post()
    {
        return Execute(HttpMethod.Post);
    }

    public (int statusCode, string response) Get()
    {
        return Execute(HttpMethod.Get);
    }

    public (int statusCode, string response) Execute()
    {
        var method = HttpMethod.Get;
        switch (_method.ToLower())
        {
            case "post": 
                method = HttpMethod.Post;
                break;
            
            case "connect": method = 
                    HttpMethod.Connect;
                break;

            case "delete": method = 
                    HttpMethod.Delete;
                break;

            case "patch": method = 
                    HttpMethod.Patch;
                break;

            case "head": method = 
                    HttpMethod.Head;
                break;

            case "put": method = 
                    HttpMethod.Put;
                break;
        }

        return Execute(method);
    }
    public (int statusCode, string response) Execute(HttpMethod method)
    {
        var url = $"{_baseUrl}";
        if (!string.IsNullOrEmpty(_queryParams))
        {
            url += $"?{_queryParams}";
        }

        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => { return true; };

        using (var client = new HttpClient(handler))
        {
            client.Timeout = TimeSpan.FromMinutes(TIMEOUT_IN_MINUTES);


            Exception? last_exception = null;
            HttpResponseMessage? resMsg = null;
            for (int i = 0; i < TRY_COUNT; i++)
            {
                try
                {
                    HttpRequestMessage msg = CreateMessage(method, url);
                    resMsg = client.Send(msg);
                    last_exception = null;
                    if (resMsg.StatusCode != (HttpStatusCode)200)
                    {
                        throw new ApiException("{1} isteginden gelen sonuc {0} Basarili sonuc alinamadi", new
                        {
                            StatusCode = resMsg.StatusCode,
                            Url = url,
                            Response = resMsg.Content
                        });
                    }

                    break;
                }
                catch (Exception ex)
                {
                    last_exception = ex;
                    Thread.Sleep(TRY_DELAY_IN_SECONDS * 1000);
                }
            }

            if (last_exception != null)
            {
                throw new ApiException("{0} kadar denendi fakat sonuc alinamadi! Gidilen adres={1} Hata Mesaji={2}", new
                {
                    deneme_sayisi = TRY_COUNT,
                    url,
                    message = last_exception.Message,
                    body = _body,
                    headers = _headers
                });
            }

            _responseMessage = resMsg;
            var response = resMsg.Content.ReadAsStringAsync().Result;

            var statuscode = (int)resMsg.StatusCode;

            return (statuscode, response);
        }
    }

    private HttpRequestMessage CreateMessage(HttpMethod method, string url)
    {
        var msg = new HttpRequestMessage(method, url);
        if (_headers != null)
            foreach (var key in _headers.Keys)
            {
                msg.Headers.Add(key, _headers[key]);
            }

        if (!string.IsNullOrWhiteSpace(_body))
        {
            var strContent = new StringContent(_body, Encoding.UTF8, _contentType);
            msg.Content = strContent;
        }

        _requestMessage = msg;
        return msg;
    }

    public Requester WithUrl(string url)
    {
        _baseUrl = url;
        return this;
    }

    public Requester WithUrlParams(params (string key, string value)[] parameters)
    {
        _queryParams = parameters.ToQueryParams();
        return this;
    }

    public Requester WithBody(string body, string contentType = "application/json")
    {
        _body = body;
        _contentType = contentType;
        return this;
    }

    public Requester WithHeaders(Dictionary<string, string> headers)
    {
        _headers = headers;
        return this;
    }

    public object Describe()
    {
        if (_requestMessage == null) throw new Exception("Request henuz calistirilmadi! Requester'i calistirin.");
        if (_responseMessage == null) throw new Exception("Response henuz olusmadi! Requester'i calistirin.");

        var reqHeaders = GetHeaders(_requestMessage.Headers, _requestMessage?.Content?.Headers);
        var resHeaders = GetHeaders(_responseMessage.Headers, _responseMessage?.Content?.Headers,
            _responseMessage.TrailingHeaders);

        return new
        {
            Request = new
            {
                Url = _requestMessage.RequestUri.ToString(),
                Method = _requestMessage.Method.ToString(),
                Content = _requestMessage?.Content?.ReadAsStringAsync().Result,
                Headers = reqHeaders
            },
            Response = new
            {
                StatusCode = (int)_responseMessage.StatusCode,
                _responseMessage.ReasonPhrase,
                Content = _responseMessage?.Content?.ReadAsStringAsync().Result,
                Headers = resHeaders
            }
        };
    }

    private Dictionary<string, object> GetHeaders(params HttpHeaders[] headers)
    {
        var result = new Dictionary<string, object>();
        foreach (var header in headers)
        {
            if (header == null) continue;
            var dict = header.ToDictionary(x => x.Key, x => x.Value);
            foreach (var d in dict)
            {
                result[d.Key] = d.Value;
            }
        }

        return result;
    }
    
    public Requester WithMethod(string httpCallMethod)
    {
        _method = httpCallMethod;
        return this;
    }

}