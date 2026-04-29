using Game.Character.Core.Stats;

public sealed class StatConfig
{
    public EStatType Type { get; }
    public float BaseValue { get; }

    // future-proof
    public StatScalingConfig Scaling { get; }

    public StatConfig(
        EStatType type,
        float baseValue,
        StatScalingConfig scaling = null)
    {
        Type = type;
        BaseValue = baseValue;
        Scaling = scaling;
    }
}