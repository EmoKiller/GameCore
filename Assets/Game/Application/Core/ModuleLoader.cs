using System;
using System.Collections.Generic;
using System.Linq;
using CustomLogger = Game.Application.Core.Logging.ICustomLogger;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Game.Application.Core
{
    /// <summary>
    /// Orchestrates dynamic module loading with dependency resolution and initialization ordering.
    /// 
    /// Responsibilities:
    /// - Register modules
    /// - Resolve module dependencies
    /// - Sort modules by initialization order
    /// - Initialize/shutdown modules safely
    /// - Detect circular dependencies
    /// 
    /// Uses ILogger abstraction for all logging (supports headless mode).
    /// 
    /// Design: Not a service locator. Direct dependency on ServiceContainer during bootstrap only.
    /// </summary>
    public class ModuleLoader
    {
        private readonly IServiceContainer _services;
        private readonly Transform _root;

        private CustomLogger _logger;

        // Registered module definitions
        private readonly List<Type> _moduleTypes = new();

        // Runtime instances
        private readonly Dictionary<Type, IGameModule> _modulesByType = new();

        // Final sorted initialization order
        private readonly List<Type> _initOrder = new();

        public ModuleLoader(IServiceContainer services, Transform root)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _root = root ?? throw new ArgumentNullException(nameof(root));
        }

        public void SetLogger(CustomLogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Register module TYPE only.
        /// Runtime instance will be created later.
        /// </summary>
        public void RegisterModule(Type moduleType)
        {
            if (moduleType == null)
                throw new ArgumentNullException(nameof(moduleType));

            if (!typeof(IGameModule).IsAssignableFrom(moduleType))
            {
                throw new Exception(
                    $"{moduleType.Name} must implement IGameModule"
                );
            }

            if (_moduleTypes.Contains(moduleType))
            {
                throw new Exception(
                    $"Module type already registered: {moduleType.Name}"
                );
            }

            _moduleTypes.Add(moduleType);
        }

        /// <summary>
        /// Create Unity module instance.
        /// </summary>
        private IGameModule CreateModule(Type type)
        {
            var go = new GameObject(type.Name);
            go.transform.SetParent(_root);

            return (IGameModule)go.AddComponent(type);
        }

        /// <summary>
        /// Main bootstrap flow.
        /// </summary>
        public async UniTask LoadModules(CancellationToken ct)
        {
            if (_moduleTypes.Count == 0)
            {
                _logger?.LogWarning("No modules registered.");
                return;
            }

            _modulesByType.Clear();
            _initOrder.Clear();

            // =====================================================
            // 1. CREATE ALL MODULE INSTANCES
            // =====================================================

            foreach (var type in _moduleTypes)
            {
                ct.ThrowIfCancellationRequested();

                var module = CreateModule(type);

                _modulesByType[type] = module;
            }

            // =====================================================
            // 2. BUILD INITIALIZATION ORDER
            // =====================================================

            var sorted = BuildInitOrder();

            _initOrder.AddRange(sorted);

            // =====================================================
            // 3. INITIALIZE IN CORRECT ORDER
            // =====================================================

            foreach (var type in _initOrder)
            {
                ct.ThrowIfCancellationRequested();

                var module = _modulesByType[type];

                _logger?.Log($"[Init] {module.ModuleName}");

                await module.InitializeAsync(_services, ct);
            }

            _logger?.Log("[Success] All modules initialized.");
        }

        /// <summary>
        /// Build dependency-based initialization order.
        /// Uses DFS topological sort.
        /// </summary>
        private List<Type> BuildInitOrder()
        {
            var result = new List<Type>();

            var visited = new HashSet<Type>();
            var visiting = new HashSet<Type>();

            void Visit(Type type)
            {
                if (visited.Contains(type))
                    return;

                if (visiting.Contains(type))
                {
                    throw new Exception(
                        $"Circular dependency detected: {type.Name}"
                    );
                }

                visiting.Add(type);

                var module = _modulesByType[type];

                var dependencies = module.GetDependencies();

                if (dependencies != null)
                {
                    foreach (var dependencyType in dependencies)
                    {
                        // Dependency provided by module
                        if (_modulesByType.ContainsKey(dependencyType))
                        {
                            Visit(dependencyType);
                        }
                        // Dependency provided by service container
                        else if (!_services.IsRegistered(dependencyType))
                        {
                            throw new Exception(
                                $"Missing dependency: {dependencyType.Name} required by {type.Name}"
                            );
                        }
                    }
                }

                visiting.Remove(type);

                visited.Add(type);

                result.Add(type);
            }

            foreach (var type in _moduleTypes)
            {
                Visit(type);
            }

            return result;
        }

        /// <summary>
        /// Shutdown all modules in reverse initialization order.
        /// </summary>
        public void ShutdownModules()
        {
            for (int i = _initOrder.Count - 1; i >= 0; i--)
            {
                var type = _initOrder[i];

                if (!_modulesByType.TryGetValue(type, out var module))
                    continue;

                try
                {
                    _logger?.Log($"[Shutdown] {module.ModuleName}");

                    module.Shutdown();
                }
                catch (Exception ex)
                {
                    _logger?.LogError(
                        $"Shutdown error in {module.ModuleName}: {ex.Message}"
                    );
                }
            }

            _modulesByType.Clear();
            _initOrder.Clear();

            _logger?.Log("All modules shutdown.");
        }

        /// <summary>
        /// Try get module by type.
        /// </summary>
        public bool TryGetModule<T>(out T module) where T : class, IGameModule
        {
            foreach (var pair in _modulesByType)
            {
                if (pair.Value is T typed)
                {
                    module = typed;
                    return true;
                }
            }

            module = null;
            return false;
        }

        /// <summary>
        /// Get all runtime module instances.
        /// </summary>
        public IReadOnlyCollection<IGameModule> GetAllModules()
        {
            return _modulesByType.Values;
        }
    }
}
