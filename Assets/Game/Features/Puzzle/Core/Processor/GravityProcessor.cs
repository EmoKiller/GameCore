using UnityEngine;

public sealed class GravityProcessor
{
    public void Apply(PuzzleBoard board, BoardChangeSet changeSet)
    {
        for (int x = 0; x < board.Width; x++)
        {
            ApplyColumn(
                board,
                x,
                changeSet);
        }
    }

    private void ApplyColumn( PuzzleBoard board, int x, BoardChangeSet changeSet)
    {
        int writeY = 0;

        for (int readY = 0;
            readY < board.Height;
            readY++)
        {
            var tile = board.Get(x, readY);

            if (tile.IsEmpty)
            {
                continue;
            }

            if (writeY != readY)
            {
                board.Set(x, writeY, tile);

                board.Set(
                    x,
                    readY,
                    new TileData(ETileType.None));

                changeSet.Add(
                    new FallTransition(
                        new TilePosition(x, readY),
                        new TilePosition(x, writeY),
                        tile.Type));
            }

            writeY++;
        }
    }
}
