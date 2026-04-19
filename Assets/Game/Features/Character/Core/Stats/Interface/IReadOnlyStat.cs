
namespace Game.Character.Core.Stats
{
    /// <summary>
    /// Defines a read-only view of a stat value with base and final (modified) values.
    /// Xác định một cái nhìn chỉ đọc về giá trị thống kê với giá trị cơ bản và cuối cùng (đã sửa đổi).
    /// </summary>
    public interface IReadOnlyStat
    {
        /// <summary>
        /// Gets the base value of the stat before any modifications are applied.
        /// Lấy giá trị cơ bản của thống kê trước khi bất kỳ sửa đổi nào được áp dụng.
        /// </summary>
        float BaseValue { get; }

        /// <summary>
        /// Gets the final value of the stat after all modifications have been applied.
        /// Lấy giá trị cuối cùng của thống kê sau khi tất cả các sửa đổi đã được áp dụng.
        /// </summary>
        float FinalValue { get; }
    }
}