using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorBomb",menuName = "Puzzle/Special/Special Behaviours/ColorBomb")]
public sealed class ColorBombBehaviour : SpecialTileBehaviour
{
    public override SpecialActivationResult Activate(
        PuzzleBoard board,
        TilePosition position,
        BoardChangeSet changeSet)
    {
        List<TilePosition> triggered =
            new List<TilePosition>();

        ETileType targetType =
            ResolveTargetType(
                board,
                position);

        if (targetType == ETileType.None)
        {
            return SpecialActivationResult.Empty();
        }

        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                TilePosition target =
                    new TilePosition(x, y);

                TileData tile =
                    board.Get(target);

                if (tile.Type != targetType)
                {
                    continue;
                }

                SpecialActivationUtility.ProcessTarget(
                    board,
                    target,
                    changeSet,
                    triggered);
            }
        }

        return new SpecialActivationResult(
            triggered);
    }

    private ETileType ResolveTargetType(
        PuzzleBoard board,
        TilePosition center)
    {
        TilePosition[] offsets =
        {
            new TilePosition(1, 0),
            new TilePosition(-1, 0),
            new TilePosition(0, 1),
            new TilePosition(0, -1),
        };

        foreach (TilePosition offset
            in offsets)
        {
            TilePosition target =
                new TilePosition(
                    center.X + offset.X,
                    center.Y + offset.Y);

            if (board.IsInside(target) == false)
            {
                continue;
            }

            TileData tile =
                board.Get(target);

            if (tile.IsEmpty)
            {
                continue;
            }

            return tile.Type;
        }

        return ETileType.None;
    }
}