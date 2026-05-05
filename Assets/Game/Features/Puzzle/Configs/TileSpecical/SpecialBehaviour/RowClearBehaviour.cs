using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Row Clear",menuName = "Puzzle/Special/Special Behaviours/Row Clear")]
public sealed class RowClearBehaviour : SpecialTileBehaviour
{
    public override SpecialActivationResult Activate(
        PuzzleBoard board,
        TilePosition position,
        BoardChangeSet changeSet)
    {
        List<TilePosition> triggered =
            new List<TilePosition>();

        for (int x = 0; x < board.Width; x++)
        {
            TilePosition target =
                new TilePosition(
                    x,
                    position.Y);

            SpecialActivationUtility.ProcessTarget(
                board,
                target,
                changeSet,
                triggered);
        }

        return new SpecialActivationResult(triggered);
    }
}
