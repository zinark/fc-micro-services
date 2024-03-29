﻿using System.Globalization;
using FCMicroservices.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace FCMicroservices.Extensions;

public static class StringExtensions
{
    private static readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Culture = CultureInfo.InvariantCulture,
        NullValueHandling = NullValueHandling.Include
    };

    public static string ToJson(this object? target, bool ident = false, int maxDepth = 64)
    {
        var formatting = ident ? Formatting.Indented : Formatting.None;
        _jsonSettings.MaxDepth = maxDepth;
        return JsonConvert.SerializeObject(target, formatting, _jsonSettings);
    }


    public static T? ParseJson<T>(this string json)
    {
        var result = JsonConvert.DeserializeObject<T>(json, _jsonSettings);
        return result;
    }
    
    public static string GetJsonValue(this string json, string path)
    {
        try
        {
            var result = JObject.Parse(json).SelectToken(path)?.ToString();
            return result;
        }
        catch (Exception e)
        {
            return path;
        }
    }
    
    public static string JoinString(this IEnumerable<string> array, string seperator = ",")
    {
        return string.Join(seperator, array);
    }


    public static (bool, T?) TryToParseJson<T>(this string json)
    {
        try
        {
            return (true, json.ParseJson<T>());
        }
        catch (Exception)
        {
            return (false, default);
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

        if (token is null) return null;
        // throw new ApiException("Gonderilen yol {0} json object icinde bulunamadi! Gonderilen {1}", new { path, json });
        if (token.Type == JTokenType.Object)
            throw new ApiException(
                "Gonderilen yol {0} json object icinde bir nesneye denk geliyor! Degeri alacak tam bir yol verin!",
                new { path });

        if (token.Type == JTokenType.Array)
            throw new ApiException(
                "Gonderilen yol {0} json object icinde bir listeye denk geliyor! Degeri alacak tam bir yol verin!",
                new { path });

        return token.Value<string>();
    }

    public static IEnumerable<T> GetCollectionFromJsonText<T>(this string json, string path) where T : new()
    {
        var jsonObject = JObject.Parse(json);
        var token = jsonObject.SelectToken(path);
        var result = new List<T>();

        if (token is null)
            throw new ApiException("Gonderilen yol {0} json object icinde bulunamadi! Gonderilen {1}",
                new { path, json });

        if (token.Type == JTokenType.Array)
        {
            if (!token.HasValues) return new List<T>();

            var rows = (token as JArray).Children();
            foreach (var row in rows)
            {
                var jsonpart = row.ToString();
                var (success, rowValue) = jsonpart.TryToParseJson<T>();

                if (!success)
                    throw new ApiException(
                        "new ile construct olabilecek bir deger gonderin! Objeye cevrilemeyen deger : {1} {0} {2}",
                        new { path, jsonpart, json });
                result.Add(rowValue);
            }

            return result;
        }

        throw new ApiException(
            "Gonderilen yol {0} json object icinde liste olmayan bir degere denk geliyor! Liste alacak tam bir yol verin!",
            new { path });
    }
}