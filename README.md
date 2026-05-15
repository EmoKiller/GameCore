GameCore - Unity Modular Framework

Luồng vận hành của GameCore

1. Giai đoạn Khởi tạo (Bootstrap)
  - File GameBootstrap.cs đóng vai trò là Composition Root. Đây là nơi mọi thứ bắt đầu Khi Scene đầu tiên khởi chạy, GameBootstrap sẽ là thành phần đầu tiên thực thi.
  - Đăng ký và khởi tạo các Service cốt lõi và khởi tạo các Module tính năng.

```csharp
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
        private void Start() 
        {
            QualitySettings.vSyncCount = 0;
            UnityEngine.Application.targetFrameRate = 120;
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
  public class PuzzleGameBootstrap : GameBootstrap
    {
        protected override void RegisterModules()
        {
            App.RegisterModule<AssetModule>();
            App.RegisterModule<UIModule>();
            App.RegisterModule<PuzzleModule>();
            App.RegisterModule<PuzzleGameplayModule>();
            App.RegisterModule<PuzzleInputModule>();
            App.RegisterModule<PuzzleGameFlowModule>();
        }
    }
```


2. Module Loader đảm nhiệm việc nạp tất cả các Module theo trình tự an toàn:
- Trước khi khởi tạo hệ thống sẽ kiểm tra phụ thuộc qua GetDependencies() để tự động sắp xếp thứ tự khởi tạo
- Tích hợp UniTask giúp việc nạp tài nguyên nặng (Assets/Data) không gây block luồng chính.

3. Game Puzzle Match-3
- Hiện tại hoàn thành Gameplay cơ bản của Match-3
- trong File BoardPreset trong cửa sổ Inspector có thể vẽ các tile trức tiếp vào Board runtime

- <img width="968" height="866" alt="Untitled" src="https://github.com/user-attachments/assets/ca93b52a-8496-4cc7-980f-7e7b67a0c20d" />

