using System.Collections.Generic;
using UnityEngine;

public sealed class GravityProcessor
{
    public void Apply(PuzzleBoard board, BoardChangeSet changeSet, HashSet<TilePosition> movedPositions)
    {
        for (int x = 0; x < board.Width; x++)
        {
            ApplyColumn(board, x, changeSet, movedPositions);
        }
    }

    private void ApplyColumn( PuzzleBoard board, int x, BoardChangeSet changeSet, HashSet<TilePosition> movedPositions)
    {
        int writeY = 0;

        for (int readY = 0; readY < board.Height; readY++)
        {
            var tile = board.Get(x, readY);

            if (tile.IsEmpty)
            {
                continue;
            }

            if (writeY != readY)
            {
                board.Set(x, writeY, tile);

                board.Set(x, readY, new TileData(ETileType.None));

                var from = new TilePosition(x, readY);
                var to = new TilePosition(x, writeY);

                changeSet.Add(new FallTransition(
                        from,
                        to,
                        tile.Type));

                movedPositions.Add(to);
            }

            writeY++;
        }
    }
}
