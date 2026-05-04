using System.Collections.Generic;
using UnityEngine;

public interface ISpecialTileResolver
{
    TileSpecialData Resolve(ETileType tileType, EMatchPatternType pattern);
}
public sealed class SpecialTileResolver : ISpecialTileResolver
{
    private readonly Dictionary<(ETileType, EMatchPatternType), TileSpecialData> _map;

    public SpecialTileResolver(SpecialResolverDatabase database)
    {
        _map = new();

        foreach (SpecialResolverEntry entry in database.Entries)
        {
            _map[(entry.TileType, entry.Pattern)] = entry.Result;
        }
    }

    public TileSpecialData Resolve(
        ETileType tileType,
        EMatchPatternType pattern)
    {
        if (_map.TryGetValue((tileType, pattern), out TileSpecialData result))
        {
            return result;
        }

        return null;
    }
}