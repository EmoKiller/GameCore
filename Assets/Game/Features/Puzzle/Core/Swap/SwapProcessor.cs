using System;
using UnityEngine;
public interface ISwapProcessor
{
    bool TrySwap( PuzzleBoard board, TilePosition a, TilePosition b, BoardChangeSet changeSet);
}
public sealed class SwapProcessor : ISwapProcessor
{
    private readonly IMatchResolver _matchResolver;

    public SwapProcessor(IMatchResolver matchResolver)
    {
        _matchResolver = matchResolver;
    }

    public bool TrySwap( PuzzleBoard board, TilePosition a, TilePosition b, BoardChangeSet changeSet)
    {
        if (board.IsInside(a) == false || board.IsInside(b) == false)
        {
            return false;
        }

        if (IsAdjacent(a, b) == false)
        {
            return false;
        }

        Swap(board, a, b);

        var matchResult = _matchResolver.Resolve(board);

        if (matchResult.HasMatches)
        {
            changeSet.Add(new SwapTransition(a, b));
            return true;
        }

        Swap(board, a, b);

        return false;
    }

    private static bool IsAdjacent(
        TilePosition a,
        TilePosition b)
    {
        int deltaX = Math.Abs(a.X - b.X);
        int deltaY = Math.Abs(a.Y - b.Y);

        return deltaX + deltaY == 1;
    }

    private static void Swap(
        PuzzleBoard board,
        TilePosition a,
        TilePosition b)
    {
        var tileA = board.Get(a);
        var tileB = board.Get(b);

        board.Set(a, tileB);
        board.Set(b, tileA);
    }
}
