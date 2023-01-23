namespace fc.micro.services.Components.CustomerDomainResolvers
{
    public class FakeTenantResolver : ITenantResolver
    {
        private string _fakeDomain;

        public FakeTenantResolver(string fakedomain = "www.test.com")
        {
            _fakeDomain = fakedomain;
        }
        public string Resolve()
        {
            _fakeDomain = _fakeDomain.Replace("www.", "");
            return _fakeDomain;
        }
    }
}
