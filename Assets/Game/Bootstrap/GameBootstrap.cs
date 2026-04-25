using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using Game.Application.Core.Input;
using Game.Application.Core.Logging;
using Game.Application.Core.Modules;
using Game.Application.Core.SceneLoader;
using Game.Application.Core.TimeService;
using Game.Application.Events;
using Game.Application.Modules.Assets;
using Game.Application.Modules.UIModules;
using UnityEngine;


namespace Game.Bootstrap
{
    /// <summary>
    /// Example bootstrap script showing how to initialize GameApplication.
    /// 
    /// This is a MonoBehaviour placed in your first scene that:
    /// 1. Creates GameApplication
    /// 2. Registers services (including ILogger and ITimeService)
    /// 3. Registers modules via factory
    /// 4. Initializes the application
    /// 
    /// After this runs, your game is fully initialized and ready.
    /// 
    /// Pattern:
    /// - THIS is your game's bootstrap
    /// - NOT a scene loader or manager
    /// - Just sets up the application infrastructure
    /// - Uses dependency injection for loose coupling
    /// </summary>
    

    
    public class GameBootstrap : MonoBehaviour
    {
        private GameApplication _app;
        public GameApplication App => _app;
        private ICustomLogger _logger;

        private CancellationTokenSource _bootstrapCts;

        private void Awake()
        {
            // Đảm bảo đối tượng này vẫn tồn tại sau khi tải lại cảnh.
            DontDestroyOnLoad(gameObject);

            _bootstrapCts = new CancellationTokenSource();
            
            // Run Task
            Run(_bootstrapCts.Token).Forget();
        }
        private async UniTask Run(CancellationToken ct)
        {
            try
            {
                // Tạo ứng dụng (đơn thể, tồn tại xuyên suốt các màn chơi)
                _app = GameApplication.Create();

                // =========================
                // Install (SYNC)
                // =========================

                // CoreServices
                RegisterCoreServices();

                // Đăng ký các Modules (thông qua factory để tách rời khỏi các lớp cụ thể)
                RegisterModules();

                // =========================
                // INITIALIZE (ASYNC)
                // =========================

                // Thao tác này gọi hàm Initialize() trên tất cả các Modules theo thứ tự. App Start
                await _app.Initialize(ct);

                //_app.Validate(); // fix sửa thêm kiểm tra có innit chưa
                _logger?.Log("=== Game Bootstrap Complete ===");
                await UniTask.Yield();

            }
            catch (OperationCanceledException)
            {
                // Bắt lỗi khi ứng dụng bị đóng giữa chừng (bình thường, không cần log error)
                _logger?.Log("Bootstrap cancelled");
            }
            catch (Exception e)
            {
                _logger?.LogError($"Bootstrap failed: {e.Message}");
                Debug.LogException(e);
            }
        }

        // Service
        private void RegisterCoreServices()
        {
            // Register ILogger FIRST - other services may depend on it
            // _logger = new UnityLogger();
            // _app.RegisterService(_logger);

            // Register the TimeService as a singleton
            var timeService = new TimeService();
            _app.RegisterService<ITimeService>(timeService);
            _app.SetTimeService(timeService);

            var lifecycle = _app.Lifecycle;
            timeService.Initialize(lifecycle);

            // Register the SceneLoader
            var sceneLoader = new SceneLoader();
            _app.RegisterService<ISceneLoader>(sceneLoader);


            _logger?.Log("Core installed");
        }

        // modules
        private void RegisterModules()
        {
            // Use factory to decouple from concrete module classes
            var moduleFactory = new ModuleFactory();

            // 0
            moduleFactory.AddModule<EventModule>(_app);
            moduleFactory.AddModule<AssetModule>(_app);
            moduleFactory.AddModule<InputModule>(_app);
            moduleFactory.AddModule<CemeraModule>(_app);

            // 1
            moduleFactory.AddModule<UIModule>(_app);

            // 10
            moduleFactory.AddModule<PlayerModule>(_app);



            // 100
            moduleFactory.AddModule<GameFlowModule>(_app);

            _logger?.Log("Modules registered successfully");
        }

        private void OnDestroy()
        {
            // Khi Bootstrapper bị hủy (hoặc tắt Game), nhấn nút "Cancel" 
            // để dừng mọi tác vụ Async đang chạy dở.
            if (_bootstrapCts != null)
            {
                _bootstrapCts.Cancel();
                _bootstrapCts.Dispose();
                _bootstrapCts = null;
            }
            // Shutdown is handled by GameApplication on quit
            // But you can manually shutdown if needed:
            if (_app != null)
            {
                _app.ShutdownApplication();
                _app = null;
                _logger = null;
            }
            
        }
        
    }
}