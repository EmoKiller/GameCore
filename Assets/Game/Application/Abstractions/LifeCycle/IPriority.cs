namespace Game.Application.Core
{
    public interface IPriority
    {
        // Số càng nhỏ (ví dụ: -100) chạy càng trước. Mặc định nên là 0.
        int Priority { get; }
    }
}