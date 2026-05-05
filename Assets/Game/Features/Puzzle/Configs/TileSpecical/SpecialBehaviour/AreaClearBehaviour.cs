using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Area Clear",menuName = "Puzzle/Special/Special Behaviours/Area Clear")]
public sealed class AreaClearBehaviour : SpecialTileBehaviour
{
    [SerializeField]
    private int _radius = 1;

    public override SpecialActivationResult Activate(
        PuzzleBoard board,
        TilePosition position,
        BoardChangeSet changeSet)
    {
        List<TilePosition> triggered =
            new List<TilePosition>();

        for (int x = -_radius; x <= _radius; x++)
        {
            for (int y = -_radius; y <= _radius; y++)
            {
                TilePosition target =
                    new TilePosition(
                        position.X + x,
                        position.Y + y);

                SpecialActivationUtility.ProcessTarget(
                    board,
                    target,
                    changeSet,
                    triggered);
            }
        }

        return new SpecialActivationResult(triggered);
    }
}