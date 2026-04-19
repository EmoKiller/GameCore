
namespace Game.Character.Core.Stats
{
    // <summary>
    /// Defines a source of stat modifiers, which can be used to track and manage modifiers applied to stats.
    /// Xác định nguồn gốc của các hệ số điều chỉnh chỉ số, có thể được sử dụng để theo dõi và quản lý các hệ số điều chỉnh được áp dụng cho các chỉ số.
    /// Ví dụ:
    /// - Equipment (weapon, armor)
    /// - Buff / Debuff
    /// - Skill
    /// - Passive ability
    /// - Environmental effect
    /// </summary>
    public interface IStatModifierSource
    {
        /// <summary>
        /// Unique identifier for debugging or tracking modifier ownership.
        /// </summary>
        string SourceId { get; }
    }
}