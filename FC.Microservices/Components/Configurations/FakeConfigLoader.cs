﻿namespace FCMicroservices.Components.Configurations
{
    public class FakeConfigLoader : IConfigLoader
    {
        public string Load(string path, string defaultValue = "")
        {
            return "yes";
        }
    }
}