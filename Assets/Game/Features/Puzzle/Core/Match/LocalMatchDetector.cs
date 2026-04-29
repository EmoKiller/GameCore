public interface ILocalMatchDetector
{
    bool HasMatchAt(IReadOnlyGrid grid, int x, int y);
}
public sealed class LocalMatchDetector : ILocalMatchDetector
{
    public bool HasMatchAt(IReadOnlyGrid grid, int x, int y)
    {
        var tile = grid.Get(x, y);

        if (tile.IsEmpty)
            return false;

        var type = tile.Type;

        return HasHorizontal(grid, x, y, type) ||
               HasVertical(grid, x, y, type);
    }

    private bool HasHorizontal(IReadOnlyGrid grid, int x, int y, ETileType type)
    {
        int count = 1;

        count += Count(grid, x, y, -1, 0, type);
        count += Count(grid, x, y, 1, 0, type);

        return count >= 3;
    }

    private bool HasVertical(IReadOnlyGrid grid, int x, int y, ETileType type)
    {
        int count = 1;

        count += Count(grid, x, y, 0, -1, type);
        count += Count(grid, x, y, 0, 1, type);

        return count >= 3;
    }

    private int Count(IReadOnlyGrid grid, int x, int y, int dx, int dy, ETileType type)
    {
        int count = 0;

        int nx = x + dx;
        int ny = y + dy;

        while (IsInside(grid, nx, ny))
        {
            var t = grid.Get(nx, ny);

            if (t.IsEmpty || t.Type != type)
                break;

            count++;
            nx += dx;
            ny += dy;
        }

        return count;
    }

    private bool IsInside(IReadOnlyGrid grid, int x, int y)
    {
        return x >= 0 && x < grid.Width &&
               y >= 0 && y < grid.Height;
    }
}