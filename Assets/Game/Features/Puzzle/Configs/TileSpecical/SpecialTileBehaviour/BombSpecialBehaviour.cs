using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bomb",menuName = "Puzzle/Special/Special Behaviours/Bomb")]
public sealed class BombSpecialBehaviour : SpecialTileBehaviour
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