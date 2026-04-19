using System;
namespace Game.Character.Core.Stats
{
    /// <summary>
    /// Defines a read-only view of a resource with current and maximum values.
    /// Xác định một cái nhìn chỉ đọc về một tài nguyên với giá trị hiện tại và tối đa.
    /// </summary>
    public interface IReadOnlyResource
    {
        /// <summary>
        /// Gets the current value of the resource.
        /// Lấy giá trị hiện tại của tài nguyên.
        /// </summary>
        float Current { get; }

        /// <summary>
        /// Gets the maximum value of the resource.
        /// Lấy giá trị tối đa của tài nguyên.
        /// </summary>
        float Max { get; }

        /// <summary>
        /// Event triggered when the resource value changes, providing the current and max values.
        /// Sự kiện được kích hoạt khi giá trị tài nguyên thay đổi, cung cấp giá trị hiện tại và tối đa.
        /// </summary>
        event Action<float, float> OnValueChanged;

        /// <summary>
        /// Event triggered when the resource reaches zero.
        /// Sự kiện được kích hoạt khi tài nguyên đạt đến số không.
        /// </summary>
        event Action OnEmpty;

    }
}