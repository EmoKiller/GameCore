using System.Collections.Generic;
using System.Linq;

public sealed class MatchCluster
{
    public ETileType TileType { get; }

    public IReadOnlyCollection<TilePosition> Positions => _positions;

    private readonly HashSet<TilePosition> _positions;

    public MatchCluster(
        ETileType tileType,
        IEnumerable<TilePosition> positions)
    {
        TileType = tileType;

        _positions = new HashSet<TilePosition>(positions);
    }

    public bool Overlaps(MatchGroup group)
    {
        if (group.TileType != TileType)
        {
            return false;
        }

        foreach (TilePosition pos in group.Positions)
        {
            if (_positions.Contains(pos))
            {
                return true;
            }
        }

        return false;
    }

    public void Merge(MatchGroup group)
    {
        foreach (TilePosition pos in group.Positions)
        {
            _positions.Add(pos);
        }
    }
}
