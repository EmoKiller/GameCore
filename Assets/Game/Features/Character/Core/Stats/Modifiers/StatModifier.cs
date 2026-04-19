
namespace Game.Character.Core.Stats
{
    /// <summary>
    /// Represents an immutable modifier that can be applied to a stat.
    /// Biểu thị một bộ điều chỉnh không thể thay đổi được có thể được áp dụng cho một thống kê.
    /// </summary>
    public sealed class StatModifier 
    {
        /// <summary>
        /// Gets the magnitude of this modifier.
        /// Lấy cường độ của bộ điều chỉnh này.
        /// </summary>
        public float Value { get; }

        /// <summary>
        /// Gets the type of modification this modifier applies.
        /// Lấy loại sửa đổi mà bộ điều chỉnh này áp dụng.
        /// </summary>
        public StatModifierType Type { get; }

        /// <summary>
        /// Gets the source that created this modifier (used for tracking and removal).
        /// Lấy nguồn đã tạo ra bộ điều chỉnh này (dùng để theo dõi và loại bỏ).
        /// </summary>
        public IStatModifierSource Source { get; }

        /// <summary>
        /// Initializes a new instance of the StatModifier class.
        /// Khởi tạo một instance mới của lớp StatModifier.
        /// </summary>
        /// <param name="value">The magnitude of the modifier.</param>
        /// <param name="type">The type of modification to apply.</param>
        /// <param name="source">The source object that created this modifier.</param>
        public StatModifier(float value, StatModifierType type, IStatModifierSource source)
        {
            Value = value;
            Type = type;
            Source = source;
        }
    }
}
