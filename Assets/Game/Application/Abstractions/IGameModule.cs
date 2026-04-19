using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Application.Core
{
    /// <summary>
    /// Hợp đồng cho các module trò chơi được quản lý bởi GameApplication.
    /// 
    /// Các module là các hệ thống con độc lập (đồ họa, nhập liệu, âm thanh, gameplay, v.v.)
    /// có thể được tải, khởi tạo, cập nhật và tắt độc lập trong khi
    /// chia sẻ các dịch vụ thông qua ServiceContainer.
    /// 
    /// Các module KHÔNG biết về nhau trực tiếp — chúng chỉ giao tiếp thông qua
    /// các dịch vụ được chia sẻ được đăng ký trong container.
    /// </summary>
    public interface IGameModule   
    {
        /// <summary>
        /// Mã định danh duy nhất cho module này.
        /// </summary>
        string ModuleName { get; }

        /// <summary>
        /// Kiểm soát thứ tự khởi tạo. Các giá trị thấp hơn khởi tạo trước.
        /// Sử dụng phạm vi 0-100. Ví dụ: Input (10), Physics (20), Gameplay (50), UI (90).
        /// </summary>
        int InitializationOrder { get; }

        /// <summary>
        /// Trả về mảng các loại mà module này phụ thuộc vào.
        /// Các dịch vụ này phải được đăng ký trước khi module khởi tạo.
        /// 
        /// Ví dụ: return new[] { typeof(IInputService), typeof(IGraphicsService) };
        /// </summary>
        Type[] GetDependencies();

        /// <summary>
        /// Được gọi một lần khi khởi động ứng dụng, sau khi tất cả các phụ thuộc được đăng ký.
        /// Khởi tạo trạng thái, đăng ký sự kiện, bắt đầu coroutine, v.v.
        /// </summary>
        /// <param name="services">ServiceContainer để giải quyết các phụ thuộc.</param>
        UniTask InitializeAsync(IServiceContainer services, CancellationToken ct = default);

        /// <summary>
        /// Được gọi khi tắt ứng dụng.
        /// Dọn dẹp tài nguyên, hủy đăng ký sự kiện, dừng coroutine, lưu trạng thái, v.v.
        /// </summary>
        void Shutdown();
    }
}
