using UnityEngine;

public sealed class StatScalingConfig
{
    public float PerLevel { get; }
    public float PerAttribute { get; }

    public StatScalingConfig(float perLevel, float perAttribute)
    {
        PerLevel = perLevel;
        PerAttribute = perAttribute;
    }
}
