namespace fc.micro.services.Components.Configurations
{
    public interface IConfigLoader
    {
        string Load(string path, string defaultValue = "");
    }
}