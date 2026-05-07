using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class RemoveMatchedTilesProcessor
{
    public void Remove(
        PuzzleBoard board,
        MatchResult matchResult,
        BoardChangeSet changeSet)
    {
        foreach (MatchCluster cluster in matchResult.Clusters)
        {
            foreach (TilePosition pos in cluster.Positions)
            {
                if (changeSet.IsProtected(pos))
                {
                    continue;
                }

                changeSet.MarkRemoved(pos);
            }
        }
        foreach (TilePosition pos in changeSet.RemovedPositions)
        {
            TileData tile = board.Get(pos);

            if (tile.IsEmpty || changeSet.IsProtected(pos))
            {
                continue;
            }

            changeSet.Add(new RemoveTransition(pos));

            board.Clear(pos);
        }
    }
}
