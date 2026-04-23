using System;
using System.Collections.Generic;
using Game.Application.Core.Logging;
using CustomLogger = Game.Application.Core.Logging.ICustomLogger;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core.TimeService;

namespace Game.Application.Core
{
    /// <summary>
    /// Điểm truy cập cấp ứng dụng cho bất kỳ trò chơi nào.
    /// 
    /// Trách nhiệm chính:
    /// - Bộ chứa dịch vụ Bootstrap
    /// - Register core services
    /// - Sắp xếp tải module 
    /// - Quản lý vòng đời ứng dụng (init, update, shutdown)
    /// - Publish lifecycle events
    /// 
    /// This is the ONLY singleton in the architecture.
    /// It exists because Unity needs a bootstrap point(Nó tồn tại vì Unity cần một điểm khởi đầu.).
    /// 
    /// Uses ILogger abstraction for all logging.
    /// This enables: headless servers, CI pipelines, mock loggers in tests.
    /// 
    /// Usage:
    ///   var app = GameApplication.Create(gameObject);
    ///   app.RegisterService(myService);
    ///   app.RegisterModule(myModule);
    ///   app.Initialize(); // Must call before game loop
    ///   // In Update(): app.Update(Time.deltaTime)
    ///   // On quit: app.Shutdown()
    /// </summary>
    public class GameApplication : MonoBehaviour
    {
        [SerializeField] private bool _logDebugInfo = true;

        private ServiceContainer _services;
        private ModuleLoader _moduleLoader;
        private ApplicationLifecycle _lifecycle;
        private ITimeService _timeService;
        private CustomLogger _logger;
        private bool _initialized = false;
        private bool _shuttingDown = false;

        private static GameApplication _instance;

        /// <summary>
        /// Create and return the GameApplication instance.
        /// Call once at game startup before doing anything else.
        /// </summary>
        public static GameApplication Create()
        {
            if (_instance != null)
            {
                Debug.LogWarning("GameApplication already exists. Returning existing instance.");
                return _instance;
            }

            var go = new GameObject("[GameApplication]");
            _instance = go.AddComponent<GameApplication>();
            DontDestroyOnLoad(go);

            Debug.Log("GameApplication: Instance created and marked as DontDestroyOnLoad.");
            return _instance;
        }

        /// <summary>
        /// Get the current GameApplication instance.
        /// </summary>
        public static GameApplication Instance => _instance;

        /// <summary>
        /// Register a service singleton.
        /// Đăng ký một đối tượng duy nhất cho dịch vụ.
        /// Phải được gọi trước khi gọi Initialize().
        /// Các dịch vụ là những khả năng cốt lõi của trò chơi (e.g. time tracking, input management, audio, etc).
        /// </summary>
        public void RegisterService<TInterface>(TInterface instance) where TInterface : IService
        {
            if (_initialized)
            {
                throw new InvalidOperationException(
                    "Cannot register services after initialization. " +
                    "Call RegisterService before Initialize()."
                );
            }

            _services.Register(instance);
            
            // Cache logger if this is ILogger being registered
            if (instance is CustomLogger logger)
            {
                _logger = logger;
                // Also pass logger to module loader for its logging
                if (_moduleLoader != null)
                {
                    _moduleLoader.SetLogger(logger);
                }
            }
            
            if (_logDebugInfo)
                _logger?.Log($"Registered service {typeof(TInterface).Name}");
        }

        /// <summary>
        /// Register a service created via factory.
        /// Must be called before Initialize().
        /// </summary>
        public void RegisterService<TInterface>(Func<IServiceContainer, TInterface> factory)
            where TInterface : IService
        {
            if (_initialized)
            {
                throw new InvalidOperationException(
                    "Cannot register services after initialization. " +
                    "Call RegisterService before Initialize()."
                );
            }

            _services.Register(factory);
            if (_logDebugInfo)
                _logger?.Log($"Registered service factory {typeof(TInterface).Name}");
        }

        /// <summary>
        /// Register a transient service.
        /// Must be called before Initialize().
        /// </summary>
        public void RegisterServiceTransient<TInterface>(Func<IServiceContainer, TInterface> factory)
            where TInterface : IService
        {
            if (_initialized)
            {
                throw new InvalidOperationException(
                    "Cannot register services after initialization. " +
                    "Call RegisterService before Initialize()."
                );
            }

            _services.RegisterTransient(factory);
            if (_logDebugInfo)
                _logger?.Log($"Registered transient service {typeof(TInterface).Name}");
        }

        /// <summary>
        /// Register a game module.
        /// Must be called before Initialize().
        /// </summary>
        public void RegisterModule(IGameModule module)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            if (_initialized)
            {
                throw new InvalidOperationException(
                    "Cannot register modules after initialization. " +
                    "Call RegisterModule before Initialize()."
                );
            }

            _moduleLoader.RegisterModule(module);
            if (_logDebugInfo)
                _logger?.Log($"Registered module '{module.ModuleName}'");
        }

        public void Validate()
        {
            // var modules = _moduleLoader.GetAllModules(); // Bạn cần thêm hàm getter này
            // foreach (var m in modules)
            // {
            //     if (!m.IsInitialized)
            //     {
            //         _logger.LogError($"[Validation] Module {m.ModuleName} chưa được khởi tạo!");
            //     }
            // }
        }

        /// <summary>
        /// Initialize the application. Must be called once before Update().
        /// 
        /// Sequence:
        /// 1. Publish PreInitialize event
        /// 2. Load all registered modules (calls Initialize() on each)
        /// 3. Publish PostInitialize event
        /// </summary>
        public async UniTask Initialize(CancellationToken ct)
        {
            if (_initialized)
            {
                _logger?.LogWarning("GameApplication already initialized.");
                return;
            }

            if (_logDebugInfo)
                _logger?.Log("Starting initialization...");

            try
            {
                _lifecycle.PublishPreInitialize();

                ct.ThrowIfCancellationRequested();
                await _moduleLoader.LoadModules(ct);
                ct.ThrowIfCancellationRequested();

                _lifecycle.PublishPostInitialize();

                _initialized = true;

                if (_logDebugInfo)
                    _logger?.Log("Initialization complete.");
            }
            catch (OperationCanceledException)
            {
                // Log dạng Warning hoặc im lặng vì đây là hành vi bình thường
                Debug.LogWarning("[CancellationRequested] Application was canceled.");
                throw; // Ném ngược ra để phía gọi (Caller) biết là task đã dừng
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Initialization failed: {ex}");
                throw;
            }
        }

        /// <summary>
        /// Get the service container.
        /// Use to resolve services after initialization.
        /// </summary>
        public IServiceContainer Services => _services;

        /// <summary>
        /// Get the application lifecycle event publisher.
        /// Subscribe to lifecycle events here.
        /// </summary>
        public IApplicationLifecycle Lifecycle => _lifecycle;

        /// <summary>
        /// Get the module loader.
        /// Use to query modules after load.
        /// </summary>
        public ModuleLoader Modules => _moduleLoader;

        public void SetTimeService(ITimeService timeService)
        {
            _timeService = timeService;
        }

        /// <summary>
        /// Check if application is initialized and running.
        /// </summary>
        public bool IsInitialized => _initialized;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize core systems
            _services = new ServiceContainer();
            _moduleLoader = new ModuleLoader(_services);
            _lifecycle = new ApplicationLifecycle();

            // Register lifecycle as a service
            _services.Register<IApplicationLifecycle>(_lifecycle);

            // Logger will be registered by client in RegisterServices()  
            // and cached via the RegisterService method override
        }

        private void Update()
        {   
            if (!_initialized || _shuttingDown)
                return;
            
            float dt = _timeService.GetTimeInfo().DeltaTime;
            _lifecycle.PublishUpdate(dt);
        }

        private void FixedUpdate()
        {
            if (!_initialized || _shuttingDown)
                return;
            
            float dt = _timeService.GetFixedTimeInfo().DeltaTime;
            _lifecycle.PublishFixedUpdate(dt);
        }

        private void LateUpdate()
        {
            if (!_initialized || _shuttingDown)
                return;
            
            float dt = _timeService.GetTimeInfo().DeltaTime;
            _lifecycle.PublishLateUpdate(dt);
        }
        private void OnDestroy()
        {
            if (_instance == this)
            {
                ShutdownApplication();
            }
        }

        /// <summary>
        /// Shutdown the application cleanly.
        /// Called automatically on app quit or scene unload.
        /// Can be called manually to shutdown gracefully.
        /// </summary>
        public void ShutdownApplication()
        {
            if (_shuttingDown)
                return;

            _shuttingDown = true;

            if (_logDebugInfo)
                _logger?.Log("Starting shutdown...");

            try
            {
                _lifecycle.PublishPreShutdown();

                if (_initialized)
                {
                    _moduleLoader.ShutdownModules();
                }

                _lifecycle.PublishPostShutdown();

                _services.Dispose();
                
                _services = null;
                _lifecycle = null;
                _timeService = null;

                if (_logDebugInfo)
                    _logger?.Log("Shutdown complete.");
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Error during shutdown: {ex}");
            }
        }
        public T InstantiateComponent<T>(GameObject prefab, Transform parent)
            where T : Component
        {
            var go = Instantiate(prefab, parent);

            if (!go.TryGetComponent<T>(out var component))
            {
                throw new Exception($"Prefab missing component: {typeof(T)}");
            }

            return component;
        }
    }
}
