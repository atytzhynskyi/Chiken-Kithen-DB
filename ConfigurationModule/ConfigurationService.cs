using System;
using System.Collections.Generic;

using jsonReadModule;

namespace ConfigurationModule
{
    public static class ConfigurationService
    { 
        public static Dictionary<string, string> Configs { get; private set; }

        public static void Init(string filePath)
        {
            Configs = JsonRead.ReadFromJson<string>(filePath);
        }
    }
}
