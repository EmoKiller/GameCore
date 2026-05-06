using System.Collections.Generic;

public readonly struct SpecialActivationResult
{
    public IReadOnlyList<TilePosition> TriggeredSpecials => _triggeredSpecials;

    private readonly List<TilePosition> _triggeredSpecials;
    public ESpecialConsumePolicy ConsumePolicy { get; }

    public bool ReTriggerNextCascade { get; }

    public SpecialActivationResult(
        List<TilePosition> triggeredSpecials,
        ESpecialConsumePolicy consumePolicy,
        bool reTriggerNextCascade = false)
    {
        _triggeredSpecials = triggeredSpecials;
        ConsumePolicy = consumePolicy;
        ReTriggerNextCascade = reTriggerNextCascade;
    }

    public static SpecialActivationResult Empty()
    {
        return new SpecialActivationResult(new List<TilePosition>(), ESpecialConsumePolicy.Destroy);
    }
}