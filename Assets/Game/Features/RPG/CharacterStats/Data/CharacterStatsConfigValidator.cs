using System;
using System.Collections.Generic;
using Game.Character.Core.Stats;
using UnityEngine;

public static class CharacterStatsConfigValidator
{
    public static void Validate(CharacterStatsConfig config)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        var statSet = new HashSet<EStatType>();

        foreach (var stat in config.Stats)
        {
            if (!statSet.Add(stat.Type))
                throw new Exception($"Duplicate stat: {stat.Type}");
        }

        foreach (var res in config.Resources)
        {
            if (!statSet.Contains(res.MaxStatType))
                throw new Exception(
                    $"Resource {res.Type} missing stat {res.MaxStatType}");
        }
    }
}
