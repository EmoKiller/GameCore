using System.Collections.Generic;
using UnityEngine;

public sealed class SpawnProcessor
{
    private readonly IRandomProvider _random;

    private static readonly ETileType[] AvailableTiles =
    {
        ETileType.Sword,
        ETileType.Heart,
        //ETileType.Shield,
        //ETileType.Coin,
    };

    public SpawnProcessor(IRandomProvider random)
    {
        _random = random;
    }

    public void FillEmpty( PuzzleBoard board, BoardChangeSet changeSet, HashSet<TilePosition> movedPositions)
    {
        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                var tile = board.Get(x, y);

                if (tile.IsEmpty == false)
                {
                    continue;
                }

                var tileType = GetRandomTile();

                board.Set(x, y, new TileData(tileType));

                var position = new TilePosition(x, y); 

                changeSet.Add(
                    new SpawnTransition(
                    position,
                    tileType
                    )
                );
                movedPositions.Add(position);
            }
        }
    }

    private ETileType GetRandomTile()
    {
        int index = _random.Range( 0, AvailableTiles.Length);

        return AvailableTiles[index];
    }
}
