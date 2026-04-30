using System;
using System.Collections.Generic;

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
        /// Current lifecycle state of the application.
        /// </summary>
        ApplicationState CurrentState { get; }

        void Register(object subscriber);
        void Unregister(object subscriber);
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
