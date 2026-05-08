using System.Collections.Generic;
using UnityEngine;

public static class SpecialActivationUtility
{
    public static void ProcessTarget(
        PuzzleBoard board,
        TilePosition target,
        BoardChangeSet changeSet,
        List<TilePosition> triggered)
    {
        if (board.IsInside(target) == false)
        {
            return;
        }

        TileData tile = board.Get(target);

        if (tile.IsEmpty || changeSet.IsRemoved(target))
        {
            return;
        }
        if (tile.HasSpecial)
        {
            triggered.Add(target);
            return;
        }

        changeSet.MarkRemoved(target);
    }
    public static List<SpecialActivationRequest> GetSpecialActivations(this MatchResult matchResult, PuzzleBoard board)
    {
        List<SpecialActivationRequest> result = new();

        foreach (TilePosition position in matchResult.AllPositions)
        {
            TileData tile = board.Get(position);

            if (tile.HasSpecial == false)
            {
                continue;
            }

            result.Add(new SpecialActivationRequest(position, tile));
        }

        return result;
    }
}