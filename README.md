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
- Trước khi khởi tạo, hệ thống truy xuất GetDependencies(). Nếu phụ thuộc chưa được nạp, hệ thống sẽ ưu tiên nạp chúng trước.
- Tích hợp UniTask giúp việc nạp tài nguyên nặng (Assets/Data) không gây block luồng chính.
- Kết hợp InitializationOrder để phân nhóm thứ tự nạp.

3. Điều phối trung tâm (GameApplication & Lifecycle)
   -GameApplication đóng vai trò là "Trái tim" của hệ thống, chịu trách nhiệm quản lý vòng đời và điều phối luồng thực thi tập trung thông qua ApplicationLifecycle
  
   -Centralized Update: GameApplication đóng vai trò là "Trái tim" của hệ thống, chịu trách nhiệm quản lý vòng đời và điều phối luồng thực thi tập trung thông qua ApplicationLifecycle.
   
   
   
   
4. Điều khiển luồng Game (GameFlow Module)
- Luồng chính của Game không nằm trong Core mà được tách ra thành một GameFlow Module riêng biệt.

- Vai trò: Đóng vai trò là "Đạo diễn", được cấu hình để khởi tạo cuối cùng sau khi tất cả các Module khác đã sẵn sàng.

- Lợi ích: Dễ dàng thay đổi kịch bản chạy game (Ví dụ: Chế độ Tutorial, Main Menu, hay Test Scene) mà không cần can thiệp vào lõi Framework.


   
   











