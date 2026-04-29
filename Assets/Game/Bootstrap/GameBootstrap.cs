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
    

    
    public abstract class GameBootstrap : MonoBehaviour
    {
        protected GameApplication App;
        protected ICustomLogger Logger;

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
                App = GameApplication.Create();

                RegisterCoreServices();
                RegisterModules();

                await App.Initialize(ct);

                Logger?.Log("=== Game Bootstrap Complete ===");
                await UniTask.Yield();
            }
            catch (OperationCanceledException)
            {
                Logger?.Log("Bootstrap cancelled");
            }
            catch (Exception e)
            {
                Logger?.LogError($"Bootstrap failed: {e}");
                Debug.LogException(e);
            }
        }

        // Service
        protected virtual void RegisterCoreServices()
        {
            // Register ILogger FIRST - other services may depend on it
            Logger = new UnityLogger();
            App.RegisterService(Logger);

            // Register the TimeService as a singleton
            var timeService = new TimeService();
            App.RegisterService<ITimeService>(timeService);
            App.SetTimeService(timeService);

            // Register the SceneLoader
            var sceneLoader = new SceneLoader();
            App.RegisterService<ISceneLoader>(sceneLoader);

            var _assetProvider = new AddressableAssetProvider(); 
            App.RegisterService<IAssetProvider>(_assetProvider);

            var eventBus = new EventBus();
            App.RegisterService<IEventBus>(eventBus);
            
            Logger?.Log("Core installed");
        }
        protected abstract void RegisterModules();

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
            if (App != null)
            {
                App.ShutdownApplication();
                App = null;
                Logger = null;
            }
            
        }
        
    }
}