using System;
using System.Collections.Generic;
using UnityEngine;
public interface IMatchEffectRegistry
{
    IMatchEffect Get(ETileType type);
}
public sealed class MatchEffectRegistry : IMatchEffectRegistry
{
    private readonly Dictionary<ETileType, IMatchEffect> _effects;

    public MatchEffectRegistry(Dictionary<ETileType, IMatchEffect> effects)
    {
        _effects = effects;
    }

    public IMatchEffect Get(ETileType type)
    {
        if (!_effects.TryGetValue(type, out var effect))
            throw new Exception($"No effect for tile type: {type}");

        return effect;
    }
}
