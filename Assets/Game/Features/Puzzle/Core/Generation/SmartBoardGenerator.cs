using System;
using UnityEngine;
public interface IBoardGenerator
{
    void Generate(IGrid grid);
}
public sealed class SmartBoardGenerator : IBoardGenerator
{
    private readonly IRandomTileProvider _tiles;
    private readonly IMoveFinder _moveFinder;

    private const int MaxTileTry = 10;

    public SmartBoardGenerator(
        IRandomTileProvider tiles,
        IMoveFinder moveFinder)
    {
        _tiles = tiles;
        _moveFinder = moveFinder;
    }

    public void Generate(IGrid grid)
    {
        FillWithoutMatches(grid);

        if (!_moveFinder.HasAnyMove(grid))
        {
            InjectGuaranteedMove(grid);
        }
    }

    // ------------------------------------------------
    // STEP 1: Fill grid without creating match
    // ------------------------------------------------

    private void FillWithoutMatches(IGrid grid)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                var tile = CreateSafeTile(grid, x, y);
                grid.Set(x, y, tile);
            }
        }
    }

    private Tile CreateSafeTile(IGrid grid, int x, int y)
    {
        for (int i = 0; i < MaxTileTry; i++)
        {
            var tile = _tiles.GetRandom();

            if (!CreatesMatch(grid, x, y, tile))
                return tile;
        }

        // fallback (extremely rare)
        return _tiles.GetRandom();
    }

    private bool CreatesMatch(IGrid grid, int x, int y, Tile tile)
    {
        var type = tile.Type;

        // horizontal check
        if (x >= 2)
        {
            var t1 = grid.Get(x - 1, y);
            var t2 = grid.Get(x - 2, y);

            if (!t1.IsEmpty && !t2.IsEmpty &&
                t1.Type == type && t2.Type == type)
                return true;
        }

        // vertical check
        if (y >= 2)
        {
            var t1 = grid.Get(x, y - 1);
            var t2 = grid.Get(x, y - 2);

            if (!t1.IsEmpty && !t2.IsEmpty &&
                t1.Type == type && t2.Type == type)
                return true;
        }

        return false;
    }

    // ------------------------------------------------
    // STEP 2: Guarantee at least one valid move
    // ------------------------------------------------

    private void InjectGuaranteedMove(IGrid grid)
    {
        // pattern: X _ X → player can swap middle to create match
        // safe position (avoid boundary)
        int x = Math.Min(1, grid.Width - 2);
        int y = Math.Min(1, grid.Height - 1);

        var tile = _tiles.GetRandom();

        grid.Set(x - 1, y, tile);
        grid.Set(x + 1, y, tile);

        // middle is different to ensure swap needed
        var different = _tiles.GetRandom();
        while (different.Type == tile.Type)
        {
            different = _tiles.GetRandom();
        }

        grid.Set(x, y, different);
    }
}
