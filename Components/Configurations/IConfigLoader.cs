namespace fc.microservices.Components.Configurations
{
    public interface IConfigLoader
    {
        string Load(string path, string defaultValue = "");
    }
}