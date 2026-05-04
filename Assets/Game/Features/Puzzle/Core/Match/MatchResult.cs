using System.Collections.Generic;
using UnityEngine;

public sealed class MatchResult
{
    public IReadOnlyList<MatchCluster> Clusters => _clusters;

    private readonly List<MatchCluster> _clusters;

    public bool HasMatches => _clusters.Count > 0;
    public IEnumerable<TilePosition> AllPositions
    {
        get
        {
            HashSet<TilePosition> result = new HashSet<TilePosition>();

            foreach (MatchCluster cluster in _clusters)
            {
                foreach (TilePosition pos in cluster.Positions)
                {
                    result.Add(pos);
                }
            }

            return result;
        }
    }

    public MatchResult( List<MatchCluster> clusters)
    {
        _clusters = clusters;
    }
    public IEnumerable<TilePosition> GetSpecialPositions(PuzzleBoard board)
    {
        foreach (MatchCluster cluster in _clusters)
        {
            foreach (TilePosition pos in cluster.Positions)
            {
                TileData tile = board.Get(pos);

                if (tile.HasSpecial)
                {
                    yield return pos;
                }
            }
        }
    }
}
