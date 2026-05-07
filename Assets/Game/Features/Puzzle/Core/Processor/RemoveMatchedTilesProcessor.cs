using System.Collections.Generic;
using UnityEngine;

public sealed class RemoveMatchedTilesProcessor
{
    public void Remove(
        PuzzleBoard board,
        MatchResult matchResult,
        BoardChangeSet changeSet)
    {
        HashSet<TilePosition> removed = new();

        foreach (MatchCluster cluster in matchResult.Clusters)
        {
            foreach (TilePosition pos in cluster.Positions)
            {
                if (removed.Contains(pos))
                {
                    continue;
                }

                removed.Add(pos);

                TileData tile = board.Get(pos);

                if (tile.IsEmpty || changeSet.IsProtected(pos) || changeSet.IsRemoved(pos))
                {
                    continue;
                }

                changeSet.MarkRemoved(pos);

                changeSet.Add(new RemoveTransition(pos));

                board.Clear(pos);
            }
        }
    }
}
