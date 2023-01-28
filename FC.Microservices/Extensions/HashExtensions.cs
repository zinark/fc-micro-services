using System.Security.Cryptography;
using System.Text;

namespace FCMicroservices.Extensions;

public static class HashExtensions
{
    public static string HASH_PREFIX = "E049DFD4E5FC3E9AC41F6E8FA7D061473ABD681E7B10A402C0E3A46425504045";
    public static string AsHash(this object? target)
    {
        
        var rawData = HASH_PREFIX + target.ToJson();
        using SHA256 hasher = SHA256.Create();
        
        byte[] bytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(rawData));

        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i].ToString("x2"));
        }

        var hash = builder.ToString().ToUpperInvariant();
        return hash;
    }
}