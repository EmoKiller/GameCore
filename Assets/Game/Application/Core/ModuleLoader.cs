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
        private readonly List<IGameModule> _allModules = new();
        private readonly Dictionary<string, IGameModule> _modulesByName = new();
        private readonly List<IUpdatable> _UpdateModules = new();
        private readonly List<IFixedUpdatable> _FixUpdateModules = new();
        private readonly List<ILateUpdatable> _LateUpdateModules = new();
        
        private CustomLogger _logger;

        public ModuleLoader(IServiceContainer services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// Set the logger for this ModuleLoader.
        /// Called by GameApplication after ILogger is registered.
        /// </summary>
        public void SetLogger(CustomLogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Register a module to be loaded.
        /// </summary>
        public void RegisterModule(IGameModule module)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            if (_modulesByName.ContainsKey(module.ModuleName))
            {
                throw new InvalidOperationException(
                    $"Module '{module.ModuleName}' already registered. " +
                    $"Module names must be unique."
                );
            }

            _allModules.Add(module);
            _modulesByName[module.ModuleName] = module;

            // Pattern Matching để phân loại vào các danh sách thực thi
            if(module is IUpdatable updatable) _UpdateModules.Add(updatable);

            if(module is IFixedUpdatable fixedUpdatable) _FixUpdateModules.Add(fixedUpdatable);

            if(module is ILateUpdatable lateUpdatable) _LateUpdateModules.Add(lateUpdatable);

        }

        /// <summary>
        /// Load all registered modules in correct dependency order.
        /// Calls Initialize() on each module.
        /// Throws on circular dependencies, missing dependencies, or initialization errors.
        /// </summary>
        public async UniTask LoadModules( CancellationToken ct)
        {
            if (_allModules.Count == 0)
            {
                _logger?.LogWarning("No modules registered.");
                return;
            }
            _initializedTypes.Clear();
            _resolvingTypes.Clear();
            _resolutionStack.Clear();
            try
            {
                // Duyệt qua danh sách gốc. 
                // Hàm ResolveAndInitialize sẽ tự lo việc nạp "cha" trước "con".
                foreach (var module in _allModules)
                {
                    await ResolveAndInitializeAsync(module, ct);
                }

                _logger?.Log("[Success] Hệ thống Module đã khởi tạo an toàn.");
            }
            catch (Exception ex)
            {
                // Bắt mọi lỗi Fatal từ đệ quy (Circular, Missing Dep, Multiple Providers)
                _logger?.LogError($"[Bootstrap Failed] {ex.Message}");
                throw; // Throw để GameApplication biết mà dừng lại
            }

            _logger?.Log($"Successfully loaded {_allModules.Count} modules.");
        }

        /// <summary>
        /// Shutdown all modules in reverse initialization order.
        /// </summary>
        public void ShutdownModules()
        {
            // Reverse order: last initialized, first shutdown
            var sortedModules = _allModules.OrderByDescending(m => m.InitializationOrder).ToList();

            foreach (var module in sortedModules)
            {
                try
                {
                    _logger?.Log($"Shutting down '{module.ModuleName}'");
                    module.Shutdown();
                }
                catch (Exception ex)
                {
                    _logger?.LogError($"Error during shutdown of '{module.ModuleName}': {ex.Message}");
                }
            }

            _allModules.Clear();
            _UpdateModules.Clear();
            _FixUpdateModules.Clear();
            _LateUpdateModules.Clear();
            _logger?.Log("All modules shutdown.");
        }

        /// <summary>
        /// Update all currently loaded modules.
        /// </summary>
        public void UpdateModules(float deltaTime)
        {
            int count = _UpdateModules.Count;
            if (count == 0) return;

            for (int i = 0; i < count; i++)
            {
                try
                {
                    _UpdateModules[i].OnUpdate(deltaTime);
                }
                catch (Exception ex)
                {
                    _logger?.LogError($"Error in Module {_UpdateModules[i].GetType().Name}: {ex.Message}");
                }
            }
        }
        public void FixUpdateModules(float deltaTime)
        {
            int count = _FixUpdateModules.Count;
            if (count == 0) return;

            for (int i = 0; i < count; i++)
            {
                try
                {
                    _FixUpdateModules[i].OnFixedUpdatable(deltaTime);
                }
                catch (Exception ex)
                {
                    _logger?.LogError($"Error in Module {_FixUpdateModules[i].GetType().Name}: {ex.Message}");
                }
            }
        }
        public void LateUpdateModules(float deltaTime)
        {
            int count = _LateUpdateModules.Count;
            if (count == 0) return;

            for (int i = 0; i < count; i++)
            {
                try
                {
                    _LateUpdateModules[i].OnLateUpdatable(deltaTime);
                }
                catch (Exception ex)
                {
                    _logger?.LogError($"Error in Module {_LateUpdateModules[i].GetType().Name}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Get a loaded module by name.
        /// </summary>
        public bool TryGetModule(string moduleName, out IGameModule module)
        {
            return _modulesByName.TryGetValue(moduleName, out module);
        }

        /// <summary>
        /// Get all loaded modules.
        /// </summary>
        public IReadOnlyList<IGameModule> GetAllModules()
        {
            return _allModules.AsReadOnly();
        }


        private readonly HashSet<Type> _initializedTypes = new();
        private readonly HashSet<Type> _resolvingTypes = new();
        private readonly Stack<Type> _resolutionStack = new();
        
        private async UniTask ResolveAndInitializeAsync(IGameModule module , CancellationToken ct)
        {
            var type = module.GetType();
            if (_initializedTypes.Contains(type)) return;

            if (!_resolvingTypes.Add(type))
            {
                throw new InvalidOperationException($"Circular dependency: {type.Name}");
            }

            _resolutionStack.Push(type);

            try
            {
                var deps = module.GetDependencies();
                if (deps != null)
                {
                    foreach (var depType in deps)
                    {
                        var provider = _allModules.FirstOrDefault(m => depType.IsAssignableFrom(m.GetType()));
                        if (provider != null)
                        {
                            // Đợi thằng cha nạp xong (Bất đồng bộ)
                            await ResolveAndInitializeAsync(provider, ct);
                        }
                        else if (!_services.IsRegistered(depType))
                        {
                            throw new Exception($"Missing dependency: {depType.Name}");
                        }
                    }
                }

                // Khởi tạo module hiện tại và CHỜ NÓ XONG THỰC SỰ
                _logger?.Log($"[Init Async] {module.ModuleName}");
                await module.InitializeAsync(_services, ct); 

                _initializedTypes.Add(type);
            }
            finally
            {
                _resolvingTypes.Remove(type);
                _resolutionStack.Pop();
            }
        }
    }
}
