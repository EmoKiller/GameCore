using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Column Clear",menuName = "Puzzle/Special/Special Behaviours/Column Clear")]
public sealed class ColumnClearBehaviour : SpecialTileBehaviour
{
    public override SpecialActivationResult Activate(
        PuzzleBoard board,
        TileData tile,
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

        return new SpecialActivationResult(triggered, ESpecialConsumePolicy.Destroy);
    }
}