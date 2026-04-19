
namespace Game.Character.Core.Stats
{
    public readonly struct CharacterStatSnapshot
    {
        public readonly float Strength;
        public readonly float Defense;
        public readonly float Agility;

        public readonly float MaxHealth;
        public readonly float MaxStamina;

        public readonly float CurrentHealth;
        public readonly float CurrentStamina;

        public CharacterStatSnapshot(
            float strength,
            float defense,
            float agility,
            float maxHealth,
            float maxStamina,
            float currentHealth,
            float currentStamina)
        {
            Strength = strength;
            Defense = defense;
            Agility = agility;

            MaxHealth = maxHealth;
            MaxStamina = maxStamina;

            CurrentHealth = currentHealth;
            CurrentStamina = currentStamina;
        }
    }  
}

