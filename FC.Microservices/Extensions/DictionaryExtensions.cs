using FCMicroservices.Components;

namespace FCMicroservices.Extensions;

public static class DictionaryExtensions
{
    public static void ValidateKeys(this IDictionary<string, object> dict, params string[] requiredKeys)
    {
        var missingKeys = dict.Keys.Except(requiredKeys);
        if (missingKeys.Count() == 0) return;
        throw new ApiException("Eksik alanlar : {0}", new { missingKeys });
    }

    public static IDictionary<string, object> ToDict(this object obj)
    {
        if (obj == null) return null;
        Dictionary<string, object> dict = new Dictionary<string, object>();

        var props = obj.GetType().GetProperties();
        foreach (var prop in props)
        {
            dict[prop.Name] = prop.GetValue(obj);
        }

        // TODO : Recursive will be implemented if needed
        return dict;
    }
}