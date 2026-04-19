using System;
using UnityEngine;
namespace Game.Application.Core.Modules
{
    /// <summary>
    /// Example config class for config-driven registration
    /// </summary>
    public class ModuleConfig
    {
        public string Name { get; set; }
        public Func<IGameModule> ModuleFactory { get; set; }
    }
}
    
