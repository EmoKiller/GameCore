using System.Collections.Generic;
using UnityEngine;

public static class SpecialActivationUtility
{
    public static void ProcessTarget(
        PuzzleBoard board,
        TilePosition position,
        BoardChangeSet changeSet,
        List<TilePosition> triggered)
    {
        if (board.IsInside(position) == false)
        {
            return;
        }

        TileData tile = board.Get(position);

        if (tile.IsEmpty)
        {
            return;
        }

        // 🔥 DEBUG
        Debug.Log($"[ProcessTarget] Hit: {position} | HasSpecial: {tile.HasSpecial}");

        // ❗ FIX 1 — respect Protect
        if (changeSet.IsProtected(position))
        {
            Debug.Log($"[ProcessTarget] SKIP Protected: {position}");
            return;
        }

        if (changeSet.IsRemoved(position))
        {
            return;
        }

        if (tile.HasSpecial)
        {
            Debug.Log($"[ProcessTarget] Chain Special: {position}");

            triggered.Add(position);

            // ❗ KHÔNG remove ngay → để chain processor xử lý
            return;
        }

        // 🔥 REMOVE NORMAL TILE
        Debug.Log($"[ProcessTarget] Remove NORMAL: {position}");

        changeSet.MarkRemoved(position);

        changeSet.Add(new RemoveTransition(position));

        board.Clear(position);
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