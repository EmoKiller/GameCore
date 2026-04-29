using System.Collections.Generic;

public interface IMatchDetector
{
    IReadOnlyList<Match> FindMatches(IGrid grid);
}
public sealed class MatchDetector : IMatchDetector
{
    public IReadOnlyList<Match> FindMatches(IGrid grid)
    {
        var results = new List<Match>();

        // Horizontal
        for (int y = 0; y < grid.Height; y++)
        {
            int count = 1;

            for (int x = 1; x < grid.Width; x++)
            {
                var current = grid.Get(x, y);
                var prev = grid.Get(x - 1, y);

                if (current.Type == prev.Type)
                {
                    count++;
                }
                else
                {
                    if (count >= 3)
                        results.Add(CreateHorizontalMatch(grid, x - count, y, count));

                    count = 1;
                }
            }

            if (count >= 3)
                results.Add(CreateHorizontalMatch(grid, grid.Width - count, y, count));
        }

        // Vertical
        for (int x = 0; x < grid.Width; x++)
        {
            int count = 1;

            for (int y = 1; y < grid.Height; y++)
            {
                var current = grid.Get(x, y);
                var prev = grid.Get(x, y - 1);

                if (current.Type == prev.Type)
                {
                    count++;
                }
                else
                {
                    if (count >= 3)
                        results.Add(CreateVerticalMatch(grid, x, y - count, count));

                    count = 1;
                }
            }

            if (count >= 3)
                results.Add(CreateVerticalMatch(grid, x, grid.Height - count, count));
        }

        return results;
    }

    private Match CreateHorizontalMatch(IGrid grid, int startX, int y, int length)
    {
        var positions = new List<(int, int)>(length);
        var type = grid.Get(startX, y).Type;

        for (int i = 0; i < length; i++)
            positions.Add((startX + i, y));

        return new Match(type, positions);
    }

    private Match CreateVerticalMatch(IGrid grid, int x, int startY, int length)
    {
        var positions = new List<(int, int)>(length);
        var type = grid.Get(x, startY).Type;

        for (int i = 0; i < length; i++)
            positions.Add((x, startY + i));

        return new Match(type, positions);
    }
}