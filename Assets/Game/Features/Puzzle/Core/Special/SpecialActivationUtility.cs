using System.Collections.Generic;

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

        TileData tile =
            board.Get(position);

        if (tile.IsEmpty)
        {
            return;
        }

        if (tile.HasSpecial)
        {
            triggered.Add(position);
        }

        if (changeSet.IsRemoved(position))
        {
            return;
        }

        bool willChain =
            tile.HasSpecial;

        changeSet.MarkRemoved(position);

        changeSet.Add(
            new RemoveTransition(position));

        if (willChain == false)
        {
            board.Clear(position);
        }
    }
}