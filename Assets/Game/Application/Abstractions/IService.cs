namespace Game.Application.Core
{
    /// <summary>
    /// Giao diện đánh dấu cho tất cả các dịch vụ được quản lý bởi ServiceContainer.
    /// Cho phép phân giải và đăng ký dịch vụ an toàn kiểu dữ liệu.
    /// 
    /// Tất cả các dịch vụ trong ứng dụng game của bạn nên triển khai giao diện này,
    /// hoặc thông qua một giao diện cụ thể hơn (ví dụ: IGameModule).
    /// </summary>
    public interface IService { }
}
