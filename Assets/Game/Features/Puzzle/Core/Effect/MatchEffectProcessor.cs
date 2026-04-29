using System.Collections.Generic;
using UnityEngine;
public interface IMatchEffectProcessor
{
    void Process(IReadOnlyList<Match> matches, ICombatContext context);
}
public sealed class MatchEffectProcessor : IMatchEffectProcessor
{
    private readonly IMatchEffectRegistry _registry;

    public MatchEffectProcessor(IMatchEffectRegistry registry)
    {
        _registry = registry;
    }

    public void Process(IReadOnlyList<Match> matches, ICombatContext context)
    {
        foreach (var match in matches)
        {
            var effect = _registry.Get(match.Type);
            effect.Apply(match, context);
        }
    }
}
