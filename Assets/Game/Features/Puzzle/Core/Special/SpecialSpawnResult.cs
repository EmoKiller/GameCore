public sealed class SpecialSpawnResult
{
    public bool HasSpecial { get; }

    public TilePosition SpawnPosition { get; }

    public ETileSpecialType SpecialType { get; }

    public SpecialSpawnResult(
        bool hasSpecial,
        TilePosition spawnPosition,
        ETileSpecialType specialType)
    {
        HasSpecial = hasSpecial;

        SpawnPosition = spawnPosition;

        SpecialType = specialType;
    }

    public static SpecialSpawnResult None()
    {
        return new SpecialSpawnResult(
            false,
            default,
            ETileSpecialType.None);
    }
}