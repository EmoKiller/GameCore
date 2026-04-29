using System;
using System.Collections.Generic;
using UnityEngine;
public interface IShuffleSystem
{
    void Shuffle(IGrid grid);
}
public sealed class ShuffleSystem : IShuffleSystem
{
    private readonly IRandomProvider _random;
    private readonly IMatchDetector _matchDetector;
    private readonly IDeadBoardDetector _deadDetector;

    private const int MaxAttempts = 50;

    public ShuffleSystem(
        IRandomProvider random,
        IMatchDetector matchDetector,
        IDeadBoardDetector deadDetector)
    {
        _random = random;
        _matchDetector = matchDetector;
        _deadDetector = deadDetector;
    }

    public void Shuffle(IGrid grid)
    {
        for (int attempt = 0; attempt < MaxAttempts; attempt++)
        {
            ShuffleInternal(grid);

            if (IsValidBoard(grid))
                return;
        }

        throw new Exception("Shuffle failed after max attempts");
    }

    private void ShuffleInternal(IGrid grid)
    {
        // flatten → shuffle → write back
        var tiles = new List<Tile>(grid.Width * grid.Height);

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                tiles.Add(grid.Get(x, y));
            }
        }

        // Fisher-Yates
        for (int i = tiles.Count - 1; i > 0; i--)
        {
            int j = _random.Next(0, i + 1);
            (tiles[i], tiles[j]) = (tiles[j], tiles[i]);
        }

        int index = 0;
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                grid.Set(x, y, tiles[index++]);
            }
        }
    }

    private bool IsValidBoard(IGrid grid)
    {
        // ❌ không có match sẵn
        if (_matchDetector.FindMatches(grid).Count > 0)
            return false;

        // ✅ có ít nhất 1 move
        if (_deadDetector.IsDead(grid))
            return false;

        return true;
    }
}
