using Microsoft.AspNetCore.Http;

namespace FCMicroservices.Components.TenantResolvers;

public class HttpTenantResolver : ITenantResolver
{
    private static readonly string[] DEFAULT_EXPECTED_HEADERS =
    {
        "X-tenant", "X-DomainName", "X-domain-name"
    };

    private readonly IHttpContextAccessor _accessor;

    public HttpTenantResolver(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string Resolve()
    {
        if (_accessor == null) return "<undefined tenant>";
        if (_accessor.HttpContext == null) return "<undefined tenant>";

        foreach (var name in DEFAULT_EXPECTED_HEADERS)
            if (_accessor.HttpContext.Request.Headers.ContainsKey(name))
                return _accessor.HttpContext.Request.Headers[name];

        return "<undefined tenant>";
    }
}