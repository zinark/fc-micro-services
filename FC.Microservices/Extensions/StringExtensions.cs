using FCMicroservices.Components.BUS;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace FCMicroservices.Extensions;

public static class StringExtensions
{
    static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public static string ToJson(this object? target, bool ident = false)
    {
        var formatting = ident ? Formatting.Indented : Formatting.None;
        return JsonConvert.SerializeObject(target, formatting, _jsonSettings);
    }

    public static T? ParseJson<T>(this string json)
    {
        var result = JsonConvert.DeserializeObject<T>(json, _jsonSettings);
        return result;
    }

    public static (bool, T?) TryToParseJson<T>(this string json)
    {
        try
        {
            return (true, json.ParseJson<T>());
        }
        catch (Exception)
        {
            return (false, default(T));
        }
    }

    public static string Dump(this string target, string title = "")
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine();
            Console.WriteLine(title);
            Console.WriteLine("=========================");
        }
        Console.WriteLine(target);
        return target;
    }

    public static bool ContainsJsonPath(this string json, string path)
    {
        var jsonObject = JObject.Parse(json);
        var token = jsonObject.SelectToken(path);
        return token != null;
    }

    public static string SetValueOfJsonText(this string json, string path, string value)
    {
        var obj = JObject.Parse(json);
        if (obj == null) return string.Empty;

        obj
            .SelectToken(path)
            .Replace(value);

        return obj.ToString();
    }

    public static string? GetValueFromJsonText(this string json, string path)
    {
        var jsonObject = JObject.Parse(json);
        var token = jsonObject.SelectToken(path);

        if (token is null)
        {
            return null;
            // throw new ApiException("Gonderilen yol {0} json object icinde bulunamadi! Gonderilen {1}", new { path, json });
        }

        if (token.Type == JTokenType.Object)
        {
            throw new ApiException("Gonderilen yol {0} json object icinde bir nesneye denk geliyor! Degeri alacak tam bir yol verin!", new { path });
        }

        if (token.Type == JTokenType.Array)
        {
            throw new ApiException("Gonderilen yol {0} json object icinde bir listeye denk geliyor! Degeri alacak tam bir yol verin!", new { path });
        }

        return token.Value<string>();
    }

    public static IEnumerable<T> GetCollectionFromJsonText<T>(this string json, string path) where T : new()
    {
        var jsonObject = JObject.Parse(json);
        var token = jsonObject.SelectToken(path);
        var result = new List<T>();

        if (token is null)
        {
            throw new ApiException("Gonderilen yol {0} json object icinde bulunamadi! Gonderilen {1}", new { path, json });
        }

        if (token.Type == JTokenType.Array)
        {
            if (!token.HasValues)
            {
                return new List<T>();
            }

            var rows = (token as JArray).Children();
            foreach (var row in rows)
            {
                string? jsonpart = row.ToString();
                (bool success, T rowValue) = jsonpart.TryToParseJson<T>();

                if (!success)
                {
                    throw new ApiException("new ile construct olabilecek bir deger gonderin! Objeye cevrilemeyen deger : {1} {0} {2}", new { path, jsonpart, json });
                }
                result.Add(rowValue);
            }
            return result;
        }

        throw new ApiException("Gonderilen yol {0} json object icinde liste olmayan bir degere denk geliyor! Liste alacak tam bir yol verin!", new { path });
    }
}
