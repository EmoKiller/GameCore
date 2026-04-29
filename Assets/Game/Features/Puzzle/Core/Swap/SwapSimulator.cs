using System;
using UnityEngine;
public interface ISwapSimulator
{
    bool WouldCreateMatch(IReadOnlyGrid grid, int x1, int y1, int x2, int y2);
}
public sealed class SwapSimulator : ISwapSimulator
{
    public bool WouldCreateMatch(IReadOnlyGrid grid, int x1, int y1, int x2, int y2)
    {
        if (!IsAdjacent(x1, y1, x2, y2))
            return false;

        var t1 = grid.Get(x1, y1);
        var t2 = grid.Get(x2, y2);

        if (t1.IsEmpty || t2.IsEmpty)
            return false;

        return Check(grid, x1, y1, t2, x2, y2, t1) ||
               Check(grid, x2, y2, t1, x1, y1, t2);
    }

    // ------------------------------------------------

    private bool Check(
        IReadOnlyGrid grid,
        int x, int y, Tile tile,
        int ox, int oy, Tile other)
    {
        var type = tile.Type;

        int Count(int dx, int dy)
        {
            int count = 0;

            int nx = x + dx;
            int ny = y + dy;

            while (nx >= 0 && nx < grid.Width &&
                   ny >= 0 && ny < grid.Height)
            {
                var t = Get(grid, nx, ny, x, y, tile, ox, oy, other);

                if (t.IsEmpty || t.Type != type)
                    break;

                count++;
                nx += dx;
                ny += dy;
            }

            return count;
        }

        int horizontal = 1 + Count(-1, 0) + Count(1, 0);
        int vertical   = 1 + Count(0, -1) + Count(0, 1);

        return horizontal >= 3 || vertical >= 3;
    }

    private Tile Get(
        IReadOnlyGrid grid,
        int px, int py,
        int x, int y, Tile tile,
        int ox, int oy, Tile other)
    {
        if (px == x && py == y) return tile;
        if (px == ox && py == oy) return other;

        return grid.Get(px, py);
    }

    private bool IsAdjacent(int x1, int y1, int x2, int y2)
        => Math.Abs(x1 - x2) + Math.Abs(y1 - y2) == 1;
}
