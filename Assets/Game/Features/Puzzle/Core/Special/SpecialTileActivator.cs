public interface ISpecialTileActivator
{
    void Activate( PuzzleBoard board, TilePosition position, BoardChangeSet changeSet);
}
public sealed class HorizontalRocketActivator : ISpecialTileActivator
{
    public void Activate( PuzzleBoard board, TilePosition position, BoardChangeSet changeSet)
    {
        for (int x = 0; x < board.Width; x++)
        {
            TilePosition target = new TilePosition(x, position.Y);

            if (board.IsEmpty(target))
            {
                continue;
            }

            changeSet.Add(new RemoveTransition(target));

            board.Clear(target);
        }
    }
}