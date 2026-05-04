using UnityEngine;

[CreateAssetMenu(fileName = "Horizontal Line",menuName = "Puzzle/Special/Special Behaviours/Horizontal Line")]
public sealed class HorizontalLineBehaviour : SpecialTileBehaviour
{
    public override void Activate(
        PuzzleBoard board,
        TilePosition position,
        BoardChangeSet changeSet)
    {
        for (int x = 0; x < board.Width; x++)
        {
            TilePosition target = new TilePosition(x, position.Y);

            if (board.IsEmpty(target))
            {
                continue;
            }

            board.Clear(target);

            changeSet.Add(
                new RemoveTransition(target));
        }
    }
}
