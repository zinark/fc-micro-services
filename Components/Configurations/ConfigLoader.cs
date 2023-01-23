using fc.micro.services.Components.BUS;

using Microsoft.Extensions.Configuration;

namespace fc.micro.services.Components.Configurations;

public class ConfigLoader : IConfigLoader
{
    readonly IConfiguration _config;
    readonly static Dictionary<string, string> _history = new();
    public static Dictionary<string, string> History => _history;
    public ConfigLoader(IConfiguration config)
    {
        _config = config;
    }

    public string Load(string path, string defaultValue = "")
    {
        var envValue = Environment.GetEnvironmentVariable(path);
        if (!string.IsNullOrWhiteSpace(envValue))
        {
            _history[path] = envValue;
            return envValue;
        }

        var connValue = _config.GetConnectionString(path);
        if (!string.IsNullOrWhiteSpace(connValue))
        {
            _history[path] = connValue;
            return connValue;
        }

        var cfgValue = _config.GetValue<string>(path);
        if (!string.IsNullOrWhiteSpace(cfgValue))
        {
            _history[path] = cfgValue;
            return cfgValue;
        }

        if (!string.IsNullOrWhiteSpace(defaultValue))
        {
            _history[path] = defaultValue;
            return defaultValue;
        }

        throw new ApiException("{0} icin sunucu environment ve config dosyasinda bir ayar bulunamadi!", path);
    }
}
