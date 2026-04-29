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

    private const int MaxIterations = 50; // safety guard

    public MatchResolutionSystem(
        IMatchDetector detector,
        IRandomTileProvider tileProvider)
    {
        _detector = detector;
        _tileProvider = tileProvider;
    }

    public IReadOnlyList<Match> Resolve(IGrid grid)
    {
        var allMatches = new List<Match>();

        for (int iteration = 0; iteration < MaxIterations; iteration++)
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

    // ------------------------------------------------
    // STEP 1: CLEAR
    // ------------------------------------------------

    private void Clear(IGrid grid, IReadOnlyList<Match> matches)
    {
        foreach (var match in matches)
        {
            foreach (var (x, y) in match.Positions)
            {
                grid.Set(x, y, Tile.Empty); // ✅ FIX CRITICAL
            }
        }
    }

    // ------------------------------------------------
    // STEP 2: GRAVITY
    // ------------------------------------------------

    private void ApplyGravity(IGrid grid)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            int writeY = 0;

            for (int y = 0; y < grid.Height; y++)
            {
                var tile = grid.Get(x, y);

                if (tile.IsEmpty)
                    continue;

                if (y != writeY)
                {
                    grid.Set(x, writeY, tile);
                    grid.Set(x, y, Tile.Empty);
                }

                writeY++;
            }
        }
    }

    // ------------------------------------------------
    // STEP 3: REFILL
    // ------------------------------------------------

    private void Refill(IGrid grid)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                if (grid.Get(x, y).IsEmpty)
                {
                    grid.Set(x, y, _tileProvider.GetRandom());
                }
            }
        }
    }
}