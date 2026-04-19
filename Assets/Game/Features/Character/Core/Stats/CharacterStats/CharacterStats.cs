using System;

namespace Game.Character.Core.Stats
{
    /// <summary>
    /// Represents the complete stat container of a character.
    /// 
    /// This class owns all primary attributes and resources such as
    /// Strength, Defense, Agility, Health, and Stamina.
    /// 
    /// It acts as the main access point for all stat-related data
    /// used by gameplay systems.
    /// </summary>
    public sealed class CharacterStats
    {
        /// <summary>
        /// Primary attributes.
        /// </summary>
        public StatValue Strength { get; }

        public StatValue Dexterity { get; }

        public StatValue Intelligence { get; }

        /// <summary>
        /// Maximum resource stats.
        /// </summary>
        public StatValue MaxHealth { get; }

        public StatValue MaxStamina { get; }

        public StatValue MaxMana { get; }

        

        /// <summary>
        /// Character resources.
        /// </summary>
        

        /// <summary>
        /// Initializes the character stats using configuration data.
        /// </summary>
        public CharacterStats(CharacterStatsConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            // Strength = new StatValue(config.Strength);
            // Defense = new StatValue(config.Defense);
            // Agility = new StatValue(config.Agility);

            MaxHealth = new StatValue(config.MaxHealth);
            // MaxStamina = new StatValue(config.MaxStamina);
        }

        /// <summary>
        /// Restores all character resources to their maximum values.
        /// </summary>
        public void RestoreAll()
        {
            
        }

        /// <summary>
        /// Returns a read-only snapshot of current stat values.
        /// Useful for UI or network synchronization.
        /// </summary>
        // public CharacterStatSnapshot CreateSnapshot()
        // {
        //     return new CharacterStatSnapshot(
        //         Strength.FinalValue,
        //         Defense.FinalValue,
        //         Agility.FinalValue,
        //         Health.Current,
        //         Health.Max,
        //         Stamina.Current,
        //         Stamina.Max
        //     );
        // }
    }
}