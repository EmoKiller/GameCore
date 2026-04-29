using UnityEngine;
namespace Game.Character.Core.Stats
{
    public interface ICharacterStatsWriter
    {
        void AddModifier(EStatType type, StatModifier modifier);
        void RemoveModifier(EStatType type, StatModifier modifier);
        void RemoveAllModifiersFromSource(IStatModifierSource source);
        void ConsumeResource(EResourceType type, float amount);
        void RestoreResource(EResourceType type, float amount);
        
    }
    internal sealed class CharacterStatsWriter : ICharacterStatsWriter
    {
        private readonly StatContainer _stats;
        private readonly ResourceContainer _resources;

        public CharacterStatsWriter(
            StatContainer stats,
            ResourceContainer resources)
        {
            _stats = stats;
            _resources = resources;
        }

        public void AddModifier(EStatType type, StatModifier modifier)
        {
            _stats.Get(type).AddModifier(modifier);
        }

        public void RemoveModifier(EStatType type, StatModifier modifier)
        {
            _stats.Get(type).RemoveModifier(modifier);
        }
        public void RemoveAllModifiersFromSource(IStatModifierSource source)
        {
            foreach (var stat in _stats.GetAll())
            {
                stat.RemoveAllFromSource(source);
            }
        }
        public void ConsumeResource(EResourceType type, float amount)
        {
            _resources.Get(type).Consume(amount);
        }

        public void RestoreResource(EResourceType type, float amount)
        {
            _resources.Get(type).Restore(amount);
        }
        
    }
}
