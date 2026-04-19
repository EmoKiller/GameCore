using System;

namespace Game.Application.Core
{
    /// <summary>
    /// Giao diện container tiêm phụ thuộc.
    /// Quản lý việc đăng ký và giải quyết dịch vụ với quyền kiểm soát thời gian tồn tại rõ ràng.
    /// 
    /// Không sử dụng mẫu định vị dịch vụ ở đây — tất cả các phụ thuộc phải được tiêm vào một cách rõ ràng.
    /// Container này chủ yếu tồn tại để GameApplication khởi tạo các dịch vụ.
    /// </summary>
    public interface IServiceContainer
    {
        /// <summary>
        /// Đăng ký một dịch vụ dưới dạng singleton (chỉ có một thể hiện duy nhất trong suốt vòng đời của ứng dụng).
        /// </summary>
        void Register<TInterface>(TInterface instance) where TInterface : IService;

        /// <summary>
        /// Đăng ký một dịch vụ để được tạo thông qua factory, dưới dạng singleton.
        /// </summary>
        void Register<TInterface>(Func<IServiceContainer, TInterface> factory) where TInterface : IService;

        /// <summary>
        /// Đăng ký một dịch vụ để được tạo mới mỗi lần giải quyết (transient).
        /// </summary>
        void RegisterTransient<TInterface>(Func<IServiceContainer, TInterface> factory) where TInterface : IService;

        /// <summary>
        /// Giải quyết một dịch vụ đã đăng ký.
        /// Ném ngoại lệ nếu dịch vụ chưa được đăng ký.
        /// </summary>
        TInterface Resolve<TInterface>() where TInterface : IService;

        /// <summary>
        /// Thử thiết lập một dịch vụ, trả về false nếu dịch vụ đó chưa được đăng ký.
        /// </summary>
        bool TryResolve<TInterface>(out TInterface service) where TInterface : IService;
        object TryResolve(Type type);

        /// <summary>
        /// Kiểm tra xem dịch vụ đã được đăng ký chưa.
        /// </summary>
        bool IsRegistered<TInterface>() where TInterface : IService;

        bool IsRegistered(Type type);
    }
}
