namespace FCMicroservices.Components.Configurations
{
    public class DefaultConfigLoader : IConfigLoader
    {
        public string Load(string? path, string defaultValue = "")
        {
            return "yes";
        }
    }
}
