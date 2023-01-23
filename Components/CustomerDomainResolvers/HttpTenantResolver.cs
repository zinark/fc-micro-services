using Microsoft.AspNetCore.Http;

namespace FCMicroservices.Components.CustomerDomainResolvers
{
    public class HttpTenantResolver : ITenantResolver
    {
        private readonly IHttpContextAccessor accessor;
        static readonly string[] DEFAULT_EXPECTED_HEADERS = new string[]
        {
            "X-tenant", "X-DomainName", "X-domain-name"
        };

        public HttpTenantResolver(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
        }

        public string Resolve()
        {
            if (accessor == null) return "<undefined tenant>";
            if (accessor.HttpContext == null) return "<undefined tenant>";

            foreach (var name in DEFAULT_EXPECTED_HEADERS)
            {
                if (accessor.HttpContext.Request.Headers.ContainsKey(name))
                {
                    return accessor.HttpContext.Request.Headers[name];
                }
            }

            return "<undefined tenant>";
        }
    }
}
