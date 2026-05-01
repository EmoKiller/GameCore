using System.Collections.Generic;
using UnityEngine;

public sealed class MatchGroup
{
    public IReadOnlyList<TilePosition> Positions => _positions;

    public ETileType TileType { get; }

    private readonly List<TilePosition> _positions;

    public MatchGroup(
        ETileType tileType,
        List<TilePosition> positions)
    {
        TileType = tileType;
        _positions = positions;
    }
}
