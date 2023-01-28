namespace FCMicroservices.Components.TenantResolvers;

public class FakeTenantResolver : ITenantResolver
{
    private string _tenant = "www-tenant-com";

    public FakeTenantResolver(string tenant = "www-tenant-com")
    {
        _tenant = tenant;
    }

    public string Resolve()
    {
        return _tenant;
    }
}