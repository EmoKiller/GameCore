using UnityEngine;

public sealed class SpawnProcessor
{
    private readonly IRandomProvider _random;

    private static readonly ETileType[] AvailableTiles =
    {
        ETileType.Sword,
        ETileType.Heart,
        ETileType.Shield,
        ETileType.Coin,
        ETileType.None
    };

    public SpawnProcessor(IRandomProvider random)
    {
        _random = random;
    }

    public void FillEmpty( PuzzleBoard board, BoardChangeSet changeSet)
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

                board.Set( x, y, new TileData(tileType));

                changeSet.Add(
                    new SpawnTransition(
                    new TilePosition(x, y),
                    tileType
                    )
                );
            }
        }
    }

    private ETileType GetRandomTile()
    {
        int index = _random.Range( 0, AvailableTiles.Length);

        return AvailableTiles[index];
    }
}
