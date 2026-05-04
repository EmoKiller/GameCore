public sealed class SpecialSpawnResult
{
    public bool HasSpecial { get; }

    public TilePosition Position  { get; }

    public EMatchPatternType Pattern { get; }

    public SpecialSpawnResult(
        bool hasSpecial,
        TilePosition position,
        EMatchPatternType pattern)
    {
        HasSpecial = hasSpecial;

        Position = position;

        Pattern = pattern;
    }

    public static SpecialSpawnResult None()
    {
        return new SpecialSpawnResult(
            false,
            default,
            EMatchPatternType.None);
    }
}