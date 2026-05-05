using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VerticalLine",menuName = "Puzzle/Special/Special Behaviours/VerticalLine")]
public sealed class VerticalLineBehaviour
    : SpecialTileBehaviour
{
    public override SpecialActivationResult Activate(
        PuzzleBoard board,
        TilePosition position,
        BoardChangeSet changeSet)
    {
        List<TilePosition> triggered =
            new List<TilePosition>();

        for (int y = 0; y < board.Height; y++)
        {
            TilePosition target =
                new TilePosition(
                    position.X,
                    y);

            SpecialActivationUtility.ProcessTarget(
                board,
                target,
                changeSet,
                triggered);
        }

        return new SpecialActivationResult(triggered);
    }
}