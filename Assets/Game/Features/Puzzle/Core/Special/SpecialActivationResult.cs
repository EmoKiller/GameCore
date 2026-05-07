using System.Collections.Generic;

public readonly struct SpecialActivationResult
{
    public IReadOnlyList<TilePosition> TriggeredSpecials => _triggeredSpecials;

    private readonly List<TilePosition> _triggeredSpecials;
    public ESpecialConsumePolicy ConsumePolicy { get; }

    public SpecialActivationResult(
        List<TilePosition> triggeredSpecials,
        ESpecialConsumePolicy consumePolicy)
    {
        _triggeredSpecials = triggeredSpecials;
        ConsumePolicy = consumePolicy;
    }

    public static SpecialActivationResult Empty()
    {
        return new SpecialActivationResult(new List<TilePosition>(), ESpecialConsumePolicy.Destroy);
    }
}