using System;

namespace Game.Application.Core
{
    /// <summary>
    /// Nhà xuất bản sự kiện cho các sự kiện vòng đời ứng dụng.
    /// Các dịch vụ và mô-đun đăng ký vào các sự kiện này để phản hồi các thay đổi trạng thái ứng dụng.
    /// Điều này giúp tách rời các module khỏi việc trực tiếp biết về GameApplication.
    /// 
    /// Mô hình: Vòng đời hướng sự kiện, không phải dựa trên hàm gọi lại.
    /// </summary>
    public interface IApplicationLifecycle : IService
    {
        /// <summary>
        /// Sự kiện này được báo cáo sau khi các dịch vụ được đăng ký nhưng trước khi các mô-đun được khởi tạo.
        /// </summary>
        event Action OnPreInitialize;

        /// <summary>
        /// Sự kiện này được báo cáo sau khi tất cả các mô-đun được khởi tạo thành công.
        /// Sử dụng sự kiện này để thiết lập giữa các mô-đun.
        /// </summary>
        event Action OnPostInitialize;

        /// <summary>
        /// Raised every frame. Subscribe to this instead of using Update() in loose services.
        /// </summary>
        event Action<float> OnUpdate;

        /// <summary>
        /// Raised at fixed intervals for physics updates. Subscribe to this instead of using FixedUpdate() in loose services.
        /// </summary>
        event Action<float> OnFixedUpdate;

        /// <summary>
        /// Raised before shutdown begins. Save state, stop long-running operations, etc.
        /// </summary>
        event Action OnPreShutdown;

        /// <summary>
        /// Raised after all shutdown is complete.
        /// Final cleanup and logging.
        /// </summary>
        event Action OnPostShutdown;

        /// <summary>
        /// Current lifecycle state of the application.
        /// </summary>
        ApplicationState CurrentState { get; }
    }

    /// <summary>
    /// Current application state in its lifecycle.
    /// </summary>
    public enum ApplicationState
    {
        /// <summary>Not yet initialized.</summary>
        PreInitialization,

        /// <summary>Services registered, ready for module initialization.</summary>
        Initializing,

        /// <summary>All modules initialized and running.</summary>
        Running,

        /// <summary>Shutdown in progress.</summary>
        ShuttingDown,

        /// <summary>Completely shut down.</summary>
        Shutdown
    }
}
