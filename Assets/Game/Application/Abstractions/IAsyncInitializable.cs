using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Game.Application.Core
{
    /// <summary>
    /// Giao diện tùy chọn dành cho các dịch vụ yêu cầu khởi tạo bất đồng bộ.
    /// 
    /// Hãy triển khai điều này cùng với IService nếu dịch vụ của bạn cần:
    /// - Tải tài nguyên không đồng bộ
    /// - Kết nối với máy chủ
    /// - Chờ hệ thống khác
    /// 
    /// Các dịch vụ triển khai IInitializable sẽ được chờ đợi trong quá trình khởi động ứng dụng.
    /// </summary>
    
    public interface IAsyncInitializable
    {
        /// <summary>
        /// Async initialization. Called during application startup.
        /// </summary>
        UniTask InitializeAsync(IServiceContainer services, CancellationToken ct = default);
    }
}
