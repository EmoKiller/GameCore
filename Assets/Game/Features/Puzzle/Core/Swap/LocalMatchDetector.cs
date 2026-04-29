public interface ILocalMatchDetector
{
    bool HasMatchAt(IGrid grid, int x, int y);
}
public sealed class LocalMatchDetector : ILocalMatchDetector
{
    public bool HasMatchAt(IGrid grid, int x, int y)
    {
        var type = grid.Get(x, y).Type;

        return HasHorizontal(grid, x, y, type) ||
               HasVertical(grid, x, y, type);
    }

    private bool HasHorizontal(IGrid grid, int x, int y, ETileType type)
    {
        int count = 1;

        count += CountDirection(grid, x, y, -1, 0, type);
        count += CountDirection(grid, x, y, 1, 0, type);

        return count >= 3;
    }

    private bool HasVertical(IGrid grid, int x, int y, ETileType type)
    {
        int count = 1;

        count += CountDirection(grid, x, y, 0, -1, type);
        count += CountDirection(grid, x, y, 0, 1, type);

        return count >= 3;
    }

    private int CountDirection(IGrid grid, int x, int y, int dx, int dy, ETileType type)
    {
        int count = 0;

        int nx = x + dx;
        int ny = y + dy;

        while (IsInside(grid, nx, ny) && grid.Get(nx, ny).Type == type)
        {
            count++;
            nx += dx;
            ny += dy;
        }

        return count;
    }

    private bool IsInside(IGrid grid, int x, int y)
    {
        return x >= 0 && x < grid.Width &&
               y >= 0 && y < grid.Height;
    }
}