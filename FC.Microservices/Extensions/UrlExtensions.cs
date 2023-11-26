using System.Collections.Specialized;
using System.Web;

namespace FCMicroservices.Extensions;

public static class UrlExtensions
{
    public static string ToQueryParams(this (string key, string val)[] parameters)
    {
        var q = HttpUtility.ParseQueryString("");
        var namevals = new NameValueCollection();
        foreach (var (key, val) in parameters)
        {
            q.Add(key, val);
        }
        return q.ToString();
    }

    public static string ParseCustomerDomain(this string host)
    {
        string[] extensions = { "com", "net", "org", "gov", "edu" }; // TODO araştırılıp düzenlenecek
        string[] splits = host.ToLowerInvariant().Split(".");
        string prevItem = "";
        string domain = "";
        if (splits.Length > 0)
        {
            foreach (var split in splits)
            {
                if (extensions.Contains(split)) domain = prevItem + "." + split;
                else if (domain.Length > 0) domain += "." + split;
                prevItem = split;
            }
        }
        return domain;
    }
}