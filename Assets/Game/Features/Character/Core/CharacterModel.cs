
using System;
using Game.Character.Core.Stats;

namespace Game.Character.Core
{
    /// <summary>
    /// CharacterModel đóng vai trò "Data Container" cho Character, chứa các thuộc tính và logic liên quan đến trạng thái của Character.
    /// Tách biệt phần dữ liệu và logic của Character khỏi phần hiển thị (View) và phần điều khiển (Controller).
    /// Single Responsibility Principle - chỉ chịu trách nhiệm quản lý dữ liệu và logic liên quan đến trạng thái của Character.
    /// </summary>
    public sealed class CharacterModel 
    {
        /// <summary>
        /// Gets the stat container of this character.
        /// </summary>
        public CharacterStats CharacterStats { get; }

        /// <summary>
        /// Gets the health resource of this character.
        /// </summary>

        public ResourceContainer Resources { get; }

        /// <summary>
        /// Triggered when the character dies (health reaches zero).
        /// </summary>
        public event Action OnDeath;

        /// <summary>
        /// Initializes a new instance of the CharacterModel.
        /// </summary>
        public CharacterModel(CharacterStatsConfig stats)
        {
            if (stats == null)
                throw new ArgumentNullException(nameof(stats));

            CharacterStats = new CharacterStats(stats);
            Resources = new ResourceContainer
            {
                { EResourceType.Health, new Health(CharacterStats.MaxHealth) }
                
            };
            Resources.Get(EResourceType.Health).OnEmpty += HandleDeath;
        }

        /// <summary>
        /// Applies damage to the character.
        /// </summary>
        public void ApplyDamage(float amount)
        {
            Resources.Get<Health>(EResourceType.Health).TakeDamage(amount);
        }

        /// <summary>
        /// Restores health to the character.
        /// </summary>
        public void ApplyHeal(float amount)
        {
            Resources.Get<Health>(EResourceType.Health).Heal(amount);
        }

        /// <summary>
        /// Consumes stamina.
        /// </summary>
        public void ApplyUseStamina(float amount)
        {
            Resources.Get(EResourceType.Stamina).Consume(amount);
        }

        /// <summary>
        /// Restores stamina.
        /// </summary>
        public void ApplyRestoreStamina(float amount)
        {
            Resources.Get(EResourceType.Stamina).Restore(amount);
        }

        public float GetMaxHealth()
        {
            return Resources.Get(EResourceType.Health).Max;
        }
        public float GetCurrentHealth()
        {
            return Resources.Get(EResourceType.Health).Current;
        }
        public bool IsAlive()
        {
            return GetCurrentHealth() > 0f;
        }

        /// <summary>
        /// Handles character death.
        /// </summary>
        private void HandleDeath()
        {
            OnDeath?.Invoke();
        }

        /// <summary>
        /// Gets a snapshot of the character's stats.
        /// </summary>
        // public CharacterStatSnapshot GetSnapshot()
        // {
        //     return new CharacterStatSnapshot(
        //         Health.Current,
        //         Health.Max,
        //         Stamina.Current,
        //         Stamina.Max,
        //         Stats.Strength.FinalValue,
        //         Stats.Defense.FinalValue,
        //         Stats.Agility.FinalValue
        //     );
        // }
    }
}
