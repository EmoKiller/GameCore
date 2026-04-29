using System.Collections.Generic;
using UnityEngine;
public interface IMatchResolutionSystem
{
    IReadOnlyList<Match> Resolve(IGrid grid);
}
public sealed class MatchResolutionSystem : IMatchResolutionSystem
{
    private readonly IMatchDetector _detector;
    private readonly IRandomTileProvider _tileProvider;

    public MatchResolutionSystem(IMatchDetector detector, IRandomTileProvider tileProvider)
    {
        _detector = detector;
        _tileProvider = tileProvider;
    }

    public IReadOnlyList<Match> Resolve(IGrid grid)
    {
        var allMatches = new List<Match>();

        while (true)
        {
            var matches = _detector.FindMatches(grid);
            if (matches.Count == 0)
                break;

            allMatches.AddRange(matches);

            Clear(grid, matches);
            ApplyGravity(grid);
            Refill(grid);
        }

        return allMatches;
    }

    private void Clear(IGrid grid, IReadOnlyList<Match> matches)
    {
        foreach (var match in matches)
        {
            foreach (var (x, y) in match.Positions)
            {
                grid.Set(x, y, default); // empty tile
            }
        }
    }

    private void ApplyGravity(IGrid grid)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            int emptyY = 0;

            for (int y = 0; y < grid.Height; y++)
            {
                var tile = grid.Get(x, y);

                if (!IsEmpty(tile))
                {
                    if (y != emptyY)
                    {
                        grid.Set(x, emptyY, tile);
                        grid.Set(x, y, default);
                    }
                    emptyY++;
                }
            }
        }
    }

    private void Refill(IGrid grid)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                if (IsEmpty(grid.Get(x, y)))
                {
                    grid.Set(x, y, _tileProvider.GetRandom());
                }
            }
        }
    }

    private bool IsEmpty(Tile tile) => tile.Equals(default);
}
