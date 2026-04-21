GameCore - Unity Modular Framework

GameCore là một Framework quản lý logic game được xây dựng nhằm mục tiêu tối ưu hóa hiệu suất, khả năng mở rộng và áp dụng tư duy Data-Oriented (Hướng dữ liệu) vào môi trường Unity truyền thống. Dự án tập trung vào việc tách biệt hoàn toàn Logic xử lý khỏi Unity Lifecycle (MonoBehaviour).

Các công nghệ sử dụng
Engine: Unity 6

Language: C#

Asynchronous: UniTask

Pattern: Composition Root, Centralized Lifecycle, State Machine, Dependency Resolution.


Luồng vận hành của GameCore

1. Giai đoạn Khởi tạo (Bootstrap)
  - File GameBootstrap.cs đóng vai trò là Composition Root. Đây là nơi mọi thứ bắt đầu Khi Scene đầu tiên khởi chạy, GameBootstrap sẽ là thành phần đầu tiên thực thi.
  - Nhiệm vụ: Đăng ký và khởi tạo các Service cốt lõi và khởi tạo các Module tính năng.
  - Mục tiêu: Đảm bảo mọi thành phần được cài đặt đồng bộ trước khi bắt đầu quá trình khởi tạo bất đồng bộ.

```csharp
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

                // Đăng ký các Modules 
                RegisterModules();

                // =========================
                // INITIALIZE (ASYNC)
                // =========================

                // Thao tác này gọi hàm Initialize() trên tất cả các Modules theo thứ tự. App Start
                await _app.Initialize(ct);

                
                _logger.Log("=== Game Bootstrap Complete ===");
                await UniTask.Yield();

            }
            catch (OperationCanceledException)
            {
                // Bắt lỗi khi ứng dụng bị đóng giữa chừng 
                _logger?.Log("Bootstrap cancelled");
            }
            catch (Exception e)
            {
                _logger?.LogError($"Bootstrap failed: {e.Message}");
                Debug.LogException(e);
            }
        }

```

- khi chạy xong các bước đăng ký (RegisterCoreServices() và RegisterModules()) bắt đầu tới bước khởi tạo _app.Initialize(ct);

```csharp
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

                await _moduleLoader.LoadModules(ct);

                _lifecycle.PublishPostInitialize();

                _initialized = true;

                if (_logDebugInfo)
                    _logger?.Log("Initialization complete.");
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Initialization failed: {ex}");
                throw;
            }
        }
```
2. Module Loader đảm nhiệm việc nạp tất cả các Module theo trình tự an toàn thông qua thuật toán đệ quy:
- Trước khi khởi tạo hệ thống truy xuất InitializationOrder để phân nhóm thứ tự nạp và GetDependencies(). Nếu phụ thuộc chưa được nạp, hệ thống sẽ ưu tiên nạp chúng trước.
- Tích hợp UniTask giúp việc nạp tài nguyên nặng (Assets/Data) không gây block luồng chính.

3. Điều phối trung tâm (GameApplication & Lifecycle)
   -GameApplication đóng vai trò là "Trái tim" của hệ thống, chịu trách nhiệm quản lý vòng đời và điều phối luồng thực thi tập trung thông qua ApplicationLifecycle
  
   -Centralized Update: GameApplication đóng vai trò là "Trái tim" của hệ thống, chịu trách nhiệm quản lý vòng đời và điều phối luồng thực thi tập trung thông qua ApplicationLifecycle.
   
   -Hiện tại hệ thống đang sử dụng Action-based cho Lifecycle. Mà hiện tại ModuleLoader đang sử dụng thêm lưu trữ các module thực thi update theo list<>. Nên sẽ tinh chỉnh lại sau .
   
   
   
   
5. Điều khiển luồng Game & Pipeline Loading
- Luồng chính của Game không nằm trong Core mà được tách ra thành một GameFlow Module riêng biệt.
- GameFlowModule đóng vai trò là đạo diễn, quản lý các trạng thái lớn của trò chơi thông qua một bộ máy trạng thái bất đồng bộ (Async State Machine).
  
- Cơ chế State Machine & Context
  - GameStateContext: Nắm giữ mọi Service quan trọng (UI, Player, Camera, SceneLoader) để cung cấp cho các trạng thái khi chuyển cảnh.
 
```csharp
public sealed class GameFlowModule : BaseGameModule ,
        IEventHandler<RequestStateChangeEvent>
    {
        private AsyncStateMachine<EGameState,GameStateContext> _stateMachine;

        private CancellationTokenSource _stateCts;
        public override string ModuleName => "GameFlowModule";

        public override int InitializationOrder => 100;

        public int Priority => EventPriority.Normal;

        public EventChannel Channel => EventChannel.System;

        public override Type[] GetDependencies()
        {
            return new Type[]
            {
                typeof(AssetModule),
                typeof(UIModule)

            };
        }
        protected override async UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct)
        {
            services.Resolve<IEventBus>().Subscribe(this); 

            var context = new GameStateContext(
                services.Resolve<ISceneLoader>(),
                services.Resolve<IUIService>(),
                services.Resolve<IEventBus>(),
                services.Resolve<IAssetProvider>(),
                services.Resolve<IPlayerService>(),
                services.Resolve<ICameraService>()
            );     
            _stateMachine = new AsyncStateMachine<EGameState, GameStateContext>(context);

            //STATES
            _stateMachine.RegisterState(EGameState.Boot, new BootState());
            _stateMachine.RegisterState(EGameState.MainMenu, new MainMenuState());
            _stateMachine.RegisterState(EGameState.Gameplay, new GameplayState());

            //Loading State
            var loadingstate = new LoadingState(
                new ILoadingOperation[]
                {
                    new MainMenuLoading(),
                    new GamePlayLoading()

                }
            );

            _stateMachine.RegisterState(EGameState.Loading, loadingstate);

            // TRANSITIONS

            _stateMachine.AddTransition(new LoadingTransition());

            // Init defaut
            await _stateMachine.SetInitialStateAsync(EGameState.Boot, ct);

            // START
            await ChangeStateAsync(EGameState.MainMenu,ct); 

        }
```

-Loading State:
  - Sẽ đọc danh sách LoadingOperation được thêm vào

```csharp
  public class LoadingState : IAsyncState<GameStateContext>
{
    private readonly Dictionary<EGameState, ILoadingOperation> _LoadingTask;

    public LoadingState(IEnumerable<ILoadingOperation> task)
    {
         _LoadingTask = task.ToDictionary(x => x.TargetState);
    }
    public async UniTask EnterAsync(GameStateContext ctx, CancellationToken ct)
    {

        Debug.Log("Enter Loading");
        Debug.Log("Next State " + ctx.NextGameState);

        await ctx.UIService.ShowAsync<LoadingView>(ct);
        
        if (_LoadingTask.TryGetValue(ctx.NextGameState, out var strategy))
        {
            Debug.Log("Start Loading");    

            await strategy.ExecuteAsync(ctx,ct);

            Debug.Log("Loading Done");

            ctx.MarkLoadingAsCompleted();

        }
        else
        {
            throw new Exception($"No loading strategy for {ctx.NextGameState}");
        }    
    }

    public async UniTask ExitAsync(GameStateContext ctx, CancellationToken ct)
    {
        Debug.Log("Exit Loading");
        await ctx.UIService.HideAsync<LoadingView>(ct);
    }

}
```
 - Loading Pipeline (Middleware Pattern)
   - khả năng lắp ráp quy trình nạp dữ liệu như một dây chuyền sản xuất: dễ dàng thêm/bớt các công đoạn loading bằng cách gọi .Use().
  

```csharp
   public class MainMenuLoading : ILoadingOperation
{
    public EGameState TargetState => EGameState.MainMenu;

    public async UniTask ExecuteAsync(GameStateContext context, CancellationToken ct)
    {
        var ctx = new LoadingContext(
            context,
            new LoadingProgressAggregator()
        );
        var pipeline = new LoadingPipeline()
            .Use(new MinimumDurationMiddleware(1.5f))
            .Use(new LoadSceneMiddleware("MainMenu"))
            .Use(new LoadAssetsMiddleware( new MainMenuAssetProvider()))
            .Use(new PrewarmMiddleware());

        await pipeline.ExecuteAsync(ctx, ct);
    }
}
public class GamePlayLoading : ILoadingOperation
{
    public EGameState TargetState => EGameState.Gameplay;

    public async UniTask ExecuteAsync(GameStateContext context, CancellationToken ct)
    {
        var ctx = new LoadingContext(
            context,
            new LoadingProgressAggregator()
        );
        var pipeline = new LoadingPipeline()
            .Use(new MinimumDurationMiddleware(1.5f))
            .Use(new LoadSceneMiddleware("Gameplay"))
            .Use(new LoadAssetsMiddleware( new GamePlayAssetProvider()))
            .Use(new PrewarmMiddleware())
            .Use(new SpawnPlayerMiddleware(context.PlayerService));

        await pipeline.ExecuteAsync(ctx, ct);
    }
}
```
   
   











