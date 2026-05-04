using System;

[Serializable]
public sealed class SpecialResolverEntry
{
    public ETileType TileType;

    public EMatchPatternType Pattern;

    public TileSpecialData Result;
}