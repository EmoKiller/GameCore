using UnityEngine;

public sealed class CharacterDefinition
{
    public string ViewAssetKey { get; }
    public CharacterStatsConfig StatsConfig { get; }

    public CharacterDefinition(
        string viewAssetKey,
        CharacterStatsConfig statsConfig)
    {
        ViewAssetKey = viewAssetKey;
        StatsConfig = statsConfig;
    }
}
