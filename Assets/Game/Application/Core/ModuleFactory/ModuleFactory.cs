using System;
using System.Collections.Generic;

namespace Game.Application.Core.Modules
{
    /// <summary>
    /// Factory for creating game modules by name.
    /// 
    /// Pragmatic implementation that:
    /// - Uses a dictionary for O(1) module lookup
    /// - Modules registered externally (not hardcoded)
    /// - Supports custom/third-party modules
    /// - Provides clear error messages if module not found
    /// - Bootstrap is agnostic of which modules exist
    /// 
    /// Modules are registered either:
    /// 1. Via RegisterModule(name, creator) calls
    /// 2. Via configuration file (JSON, YAML) in future
    /// </summary>
    public class ModuleFactory : IModuleFactory
    {
        private readonly Dictionary<string, Func<IGameModule>> _moduleCreators;

        public ModuleFactory()
        {
            // Empty by default - modules are registered by client
            _moduleCreators = new();
        }

        /// <summary>
        /// Adds a custom module to the factory.
        /// Allows games to extend modules without modifying core factory.
        /// </summary>
        public void RegisterModule(string name, Func<IGameModule> creator)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Module name cannot be empty", nameof(name));

            if (creator == null)
                throw new ArgumentNullException(nameof(creator));

            _moduleCreators[name.ToLower()] = creator;
        }

        public IGameModule CreateModule(string moduleName)
        {
            if (string.IsNullOrEmpty(moduleName))
                throw new ArgumentException("Module name cannot be empty", nameof(moduleName));

            var key = moduleName.ToLower();

            if (!_moduleCreators.TryGetValue(key, out var creator))
            {
                var available = string.Join(", ", _moduleCreators.Keys);
                throw new InvalidOperationException(
                    $"Module '{moduleName}' not found in factory.\n" +
                    $"Available modules: {available}\n" +
                    $"Did you forget to RegisterModule()?");
            }

            try
            {
                return creator();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to create module '{moduleName}'. See inner exception.", ex);
            }
        }
    }

    
}
