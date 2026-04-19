using System;

namespace Game.Character.Core.Stats
{
    /// <summary>
    /// Represents the health resource of a character.
    /// </summary>
    public sealed class Health : Resource
    {
        /// <summary>
        /// Triggered when the character takes damage.
        /// Kích hoạt khi nhân vật nhận sát thương.
        /// </summary>
        public event Action<float> OnDamaged;

        /// <summary>
        /// Triggered when the character is healed.
        /// Kích hoạt khi nhân vật được hồi máu.
        /// </summary>
        public event Action<float> OnHealed;

        /// <summary>
        /// Initializes a new instance of the Health class.
        /// </summary>
        /// <param name="maxHealth">The maximum health value.</param>
        public Health(StatValue maxHealth) : base(maxHealth)
        {
        }

        /// <summary>
        /// Applies damage to the health resource.
        /// Áp dụng sát thương lên máu.
        /// </summary>
        public void TakeDamage(float amount)
        {
            if (amount <= 0f)
                return;

            OnDamaged?.Invoke(amount);

            Consume(amount);
        }

        /// <summary>
        /// Restores health.
        /// Hồi máu.
        /// </summary>
        public void Heal(float amount)
        {
            if (amount <= 0f)
                return;

            OnHealed?.Invoke(amount);

            Restore(amount);
        }

        /// <summary>
        /// Instantly restores health to maximum.
        /// Hồi máu đầy.
        /// </summary>
        public void FullRestore()
        {
            SetCurrent(Max);
        }
    }
}
