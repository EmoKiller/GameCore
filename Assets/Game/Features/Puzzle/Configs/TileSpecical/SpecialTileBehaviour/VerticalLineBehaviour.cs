using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vertical Line",menuName = "Puzzle/Special/Special Behaviours/Vertical Line")]
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