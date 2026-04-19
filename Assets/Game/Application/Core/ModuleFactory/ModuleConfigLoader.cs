using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
namespace Game.Application.Core.Modules
{
    public static class ModuleConfigLoader
    {
        public static IEnumerable<ModuleConfig> LoadFromJson(string jsonPath)
        {
            if (!File.Exists(jsonPath))
                throw new FileNotFoundException("Module config file not found", jsonPath);

            var jsonText = File.ReadAllText(jsonPath);

            // Dùng Newtonsoft.Json để deserialize
            var rawConfigs = JsonConvert.DeserializeObject<List<RawModuleConfig>>(jsonText);
            if (rawConfigs == null)
                throw new InvalidOperationException("Failed to parse module config JSON.");

            var configs = new List<ModuleConfig>();

            foreach (var raw in rawConfigs)
            {
                configs.Add(new ModuleConfig
                {
                    Name = raw.Name,
                    ModuleFactory = () =>
                    {
                        var type = Type.GetType(raw.Type);
                        if (type == null)
                            throw new InvalidOperationException($"Module type '{raw.Type}' not found.");

                        if (!typeof(IGameModule).IsAssignableFrom(type))
                            throw new InvalidOperationException($"Type '{raw.Type}' does not implement IGameModule.");

                        return (IGameModule)Activator.CreateInstance(type)!;
                    }
                });
            }

            return configs;
        }

        private class RawModuleConfig
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }
    }
}
