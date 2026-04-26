using UnityEngine;
namespace Game.Character.Core.Stats
{   
    public struct BuffModifier
    {
        public EStatType StatType;
        public float Value;
        public StatModifierType ModifierType;
    }
    public sealed class BuffDefinition
    {
        public readonly float Duration;
        public readonly BuffModifier[] Modifiers;

        public readonly EBuffStackingPolicy StackingPolicy;
        public readonly int MaxStacks;

        public BuffDefinition(
            float duration,
            BuffModifier[] modifiers,
            EBuffStackingPolicy stackingPolicy,
            int maxStacks = 1)
        {
            Duration = duration;
            Modifiers = modifiers;
            StackingPolicy = stackingPolicy;
        MaxStacks = maxStacks;
    }
}
}
