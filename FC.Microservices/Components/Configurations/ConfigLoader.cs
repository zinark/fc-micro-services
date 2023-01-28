using FCMicroservices.Components.BUS;
using Microsoft.Extensions.Configuration;

namespace FCMicroservices.Components.Configurations;

public class ConfigLoader : IConfigLoader
{
    private readonly IConfiguration _config;

    public ConfigLoader(IConfiguration config)
    {
        _config = config;
    }

    public static Dictionary<string?, string> History { get; } = new();

    public string Load(string? path, string defaultValue = "")
    {
        var envValue = Environment.GetEnvironmentVariable(path);
        if (!string.IsNullOrWhiteSpace(envValue))
        {
            History[path] = envValue;
            return envValue;
        }

        var connValue = _config.GetConnectionString(path);
        if (!string.IsNullOrWhiteSpace(connValue))
        {
            History[path] = connValue;
            return connValue;
        }

        var cfgValue = _config.GetValue<string>(path);
        if (!string.IsNullOrWhiteSpace(cfgValue))
        {
            History[path] = cfgValue;
            return cfgValue;
        }

        if (!string.IsNullOrWhiteSpace(defaultValue))
        {
            History[path] = defaultValue;
            return defaultValue;
        }

        throw new ApiException("{0} icin sunucu environment ve config dosyasinda bir ayar bulunamadi!", path);
    }
}