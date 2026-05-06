using System.Collections.Generic;

public sealed class SpecialChainProcessResult
{
    public IReadOnlyList<TileData> PersistentTiles { get; }

    public SpecialChainProcessResult(
        List<TileData> persistentTiles)
    {
        PersistentTiles = persistentTiles;
    }
}
