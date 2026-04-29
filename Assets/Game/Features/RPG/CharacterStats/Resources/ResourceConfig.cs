using Game.Character.Core.Stats;

public sealed class ResourceConfig
{
    public EResourceType Type { get; }

    public EStatType MaxStatType { get; }

    public float InitialValueRatio { get; }
    
    public EResourceBehaviorFlags Behavior { get; }
    public ResourceRegenConfig Regen { get; }

    public ResourceConfig(
        EResourceType type,
        EStatType maxStatType,
        float initialValueRatio = 1f,
        EResourceBehaviorFlags behavior = EResourceBehaviorFlags.None,
        ResourceRegenConfig regen = null
    )
    {
        Type = type;
        MaxStatType = maxStatType;
        InitialValueRatio = initialValueRatio;
        Behavior = behavior;
        Regen = regen;
    }
}