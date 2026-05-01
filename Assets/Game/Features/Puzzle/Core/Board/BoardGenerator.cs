using UnityEngine;

public sealed class BoardGenerator
{
    private readonly IRandomProvider _random;

    private static readonly ETileType[] AvailableTiles =
    {
        ETileType.Sword,
        ETileType.Heart,
        ETileType.Shield,
        //ETileType.Coin
    };

    public BoardGenerator(IRandomProvider random)
    {
        _random = random;
    }

    public void Fill(PuzzleBoard board)
    {
        for (int y = 0; y < board.Height; y++)
        {
            for (int x = 0; x < board.Width; x++)
            {
                var tile = GenerateTile(board, x, y);

                board.Set(x, y, new TileData(tile));
            }
        }
    }

    private ETileType GenerateTile(
        PuzzleBoard board,
        int x,
        int y)
    {
        while (true)
        {
            var randomTile = GetRandomTile();

            if (CreatesMatch(board, x, y, randomTile))
            {
                continue;
            }

            return randomTile;
        }
    }

    private ETileType GetRandomTile()
    {
        var index = _random.Range(0, AvailableTiles.Length);

        return AvailableTiles[index];
    }

    private bool CreatesMatch(
        PuzzleBoard board,
        int x,
        int y,
        ETileType tileType)
    {
        // Horizontal
        if (x >= 2)
        {
            var leftA = board.Get(x - 1, y);
            var leftB = board.Get(x - 2, y);

            if (leftA.Type == tileType &&
                leftB.Type == tileType)
            {
                return true;
            }
        }

        // Vertical
        if (y >= 2)
        {
            var downA = board.Get(x, y - 1);
            var downB = board.Get(x, y - 2);

            if (downA.Type == tileType &&
                downB.Type == tileType)
            {
                return true;
            }
        }

        return false;
    }
}
