namespace FCMicroservices.Components.Configurations;

public class TestableConfigLoader : IConfigLoader
{
    private Dictionary<string, string> _values = new();

    public string Load(string? path, string? defaultValue = null)
    {
        if (_values.ContainsKey(path))
        {
            return _values[path];
        }

        return defaultValue ?? "no";
    }

    public TestableConfigLoader AddConfig(string path, string value)
    {
        _values[path] = value;
        return this;
    }
}