using System.Collections.Generic;
using UnityEngine;

public sealed class RemoveMatchedTilesProcessor
{
    public void Remove( PuzzleBoard board, MatchResult matchResult, BoardChangeSet changeSet)
    {
        // HashSet<TilePosition> removed = new HashSet<TilePosition>();
        // foreach (var group in matchResult.Groups)
        // {
        //     foreach (var position in group.Positions)
        //     {
        //         if (removed.Add(position) == false)
        //         {
        //             continue;
        //         }
        //         changeSet.Add(
        //             new RemoveTransition(position));

        //         board.Set(
        //             position,
        //             new TileData(ETileType.None));
        //     }
        // }
        foreach (var group in matchResult.Clusters)
        {
            foreach (var position in group.Positions)
            {
                if (changeSet.IsProtected(position))
                {
                    continue;
                }
                
                changeSet.Add(new RemoveTransition(position));

                board.Clear(position);
            }
        }
    }
}
