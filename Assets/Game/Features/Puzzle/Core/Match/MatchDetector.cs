using System.Collections.Generic;

public interface IMatchDetector
{
    IReadOnlyList<Match> FindMatches(IReadOnlyGrid grid);
}
public sealed class MatchDetector : IMatchDetector
{
    public IReadOnlyList<Match> FindMatches(IReadOnlyGrid grid)
    {
        var results = new List<Match>();

        ScanHorizontal(grid, results);
        ScanVertical(grid, results);

        return results;
    }

    // ------------------------------------------------

    private void ScanHorizontal(IReadOnlyGrid grid, List<Match> results)
    {
        for (int y = 0; y < grid.Height; y++)
        {
            int count = 1;

            for (int x = 1; x < grid.Width; x++)
            {
                var current = grid.Get(x, y);
                var prev = grid.Get(x - 1, y);

                if (!current.IsEmpty &&
                    !prev.IsEmpty &&
                    current.Type == prev.Type)
                {
                    count++;
                }
                else
                {
                    if (count >= 3)
                        results.Add(CreateHorizontal(grid, x - count, y, count));

                    count = 1;
                }
            }

            if (count >= 3)
                results.Add(CreateHorizontal(grid, grid.Width - count, y, count));
        }
    }

    private void ScanVertical(IReadOnlyGrid grid, List<Match> results)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            int count = 1;

            for (int y = 1; y < grid.Height; y++)
            {
                var current = grid.Get(x, y);
                var prev = grid.Get(x, y - 1);

                if (!current.IsEmpty &&
                    !prev.IsEmpty &&
                    current.Type == prev.Type)
                {
                    count++;
                }
                else
                {
                    if (count >= 3)
                        results.Add(CreateVertical(grid, x, y - count, count));

                    count = 1;
                }
            }

            if (count >= 3)
                results.Add(CreateVertical(grid, x, grid.Height - count, count));
        }
    }

    // ------------------------------------------------

    private Match CreateHorizontal(IReadOnlyGrid grid, int startX, int y, int length)
    {
        var positions = new List<(int, int)>(length);
        var type = grid.Get(startX, y).Type;

        for (int i = 0; i < length; i++)
            positions.Add((startX + i, y));

        return new Match(type, positions);
    }

    private Match CreateVertical(IReadOnlyGrid grid, int x, int startY, int length)
    {
        var positions = new List<(int, int)>(length);
        var type = grid.Get(x, startY).Type;

        for (int i = 0; i < length; i++)
            positions.Add((x, startY + i));

        return new Match(type, positions);
    }
}