using UnityEngine;

public sealed class RemoveMatchedTilesProcessor
{
    public void Remove( PuzzleBoard board, MatchResult matchResult, BoardChangeSet changeSet)
    {
        foreach (var group in matchResult.Groups)
        {
            foreach (var position in group.Positions)
            {
                changeSet.Add(
                    new RemoveTransition(position));

                board.Set(
                    position,
                    new TileData(ETileType.None));
            }
        }
    }
}
