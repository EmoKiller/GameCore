using System;
using System.Collections.Generic;
using Game.Application.Configuration.BaseScriptableObject;
using UnityEngine;

public class CharacterStatsConfig 
{
    public IReadOnlyList<StatConfig> Stats { get; }
    public IReadOnlyList<ResourceConfig> Resources { get; }
    public CharacterStatsConfig(
        IReadOnlyList<StatConfig> stats,
        IReadOnlyList<ResourceConfig> resources)
    {
        Stats = stats ?? throw new ArgumentNullException(nameof(stats));
        Resources = resources ?? throw new ArgumentNullException(nameof(resources));
    }
}
