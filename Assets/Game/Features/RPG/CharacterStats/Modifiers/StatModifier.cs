
namespace Game.Character.Core.Stats
{
    /// <summary>
    /// Represents an immutable modifier that can be applied to a stat.
    /// Biểu thị một bộ điều chỉnh không thể thay đổi được có thể được áp dụng cho một thống kê.
    /// </summary>
    public sealed class StatModifier
    {
        public float Value { get; }
        public StatModifierType Type { get; }
        public int Priority { get; }
        public IStatModifierSource Source { get; }

        public StatModifier(
            float value,
            StatModifierType type,
            IStatModifierSource source,
            int priority = 0)
        {
            Value = value;
            Type = type;
            Source = source;
            Priority = priority;
        }
    }
}
