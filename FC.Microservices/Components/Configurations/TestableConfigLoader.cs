namespace FCMicroservices.Components.Configurations;

public class TestableConfigLoader : IConfigLoader
{
    public string Load(string? path, string? defaultValue = null)
    {
        return defaultValue ?? "no";
    }
}