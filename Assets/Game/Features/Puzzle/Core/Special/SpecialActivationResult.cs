using System.Collections.Generic;

public readonly struct SpecialActivationResult
{
    public IReadOnlyList<TilePosition> TriggeredSpecials => _triggeredSpecials;

    private readonly List<TilePosition> _triggeredSpecials;

    public SpecialActivationResult(List<TilePosition> triggeredSpecials)
    {
        _triggeredSpecials = triggeredSpecials;
    }

    public static SpecialActivationResult Empty()
    {
        return new SpecialActivationResult(new List<TilePosition>());
    }
}