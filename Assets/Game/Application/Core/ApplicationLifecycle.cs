using System;

namespace Game.Application.Core
{
    /// <summary>
    /// Triển khai trình phát sự kiện vòng đời ứng dụng.
    /// 
    /// Dịch vụ này công bố các sự kiện tại những thời điểm quan trọng trong vòng đời của ứng dụng.,
    /// cho phép các mô-đun và dịch vụ phản ứng mà không cần phụ thuộc trực tiếp.
    /// 
    /// Mô hình: Mô hình quan sát dựa trên sự kiện.
    /// Cách sử dụng: Đăng ký nhận thông báo về các sự kiện quan trọng đối với hệ thống của bạn.
    /// </summary>
    public class ApplicationLifecycle : IApplicationLifecycle
    {
        private ApplicationState _currentState = ApplicationState.PreInitialization;

        public ApplicationState CurrentState => _currentState;

        public event Action OnPreInitialize;
        public event Action OnPostInitialize;
        public event Action<float> OnUpdate;
        public event Action<float> OnFixedUpdate;
        public event Action OnPreShutdown;
        public event Action OnPostShutdown;

        /// <summary>
        /// Được GameApplication gọi trong giai đoạn khởi tạo 1.
        /// </summary>
        internal void PublishPreInitialize()
        {
            _currentState = ApplicationState.Initializing;
            OnPreInitialize?.Invoke();
        }

        /// <summary>
        /// Được GameApplication gọi sau khi các modules được khởi tạo.
        /// </summary>
        internal void PublishPostInitialize()
        {
            _currentState = ApplicationState.Running;
            OnPostInitialize?.Invoke();
        }

        /// <summary>
        /// Được GameApplication gọi trong mỗi khung hình.
        /// </summary>
        internal void PublishUpdate(float deltaTime)
        {
            if (_currentState == ApplicationState.Running)
                OnUpdate?.Invoke(deltaTime);
        }

        internal void PublishFixedUpdate(float fixedDeltaTime)
        {
            if (_currentState == ApplicationState.Running)
                OnFixedUpdate?.Invoke(fixedDeltaTime);
        }
        /// <summary>
        /// Được gọi bởi GameApplication trong giai đoạn tắt máy 1.
        /// </summary>
        internal void PublishPreShutdown()
        {
            _currentState = ApplicationState.ShuttingDown;
            OnPreShutdown?.Invoke();
        }

        /// <summary>
        /// Được gọi bởi GameApplication sau khi tắt máy hoàn tất.
        /// </summary>
        internal void PublishPostShutdown()
        {
            _currentState = ApplicationState.Shutdown;
            OnPostShutdown?.Invoke();
        }
    }
}
