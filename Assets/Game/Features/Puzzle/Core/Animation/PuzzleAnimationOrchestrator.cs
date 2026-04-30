using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class PuzzleAnimationOrchestrator
{
    private readonly BoardView _board;
    private readonly BoardAnimationController _anim;
    private readonly IPuzzleInteractionLock _lock;

    public PuzzleAnimationOrchestrator(
        BoardView board,
        BoardAnimationController anim,
        IPuzzleInteractionLock interactionLock)
    {
        _board = board;
        _anim = anim;
        _lock = interactionLock;
    }

    public async UniTask PlaySwapAsync(SwapCommand cmd)
    {
        _lock.Lock();

        var a = _board.GetTile(cmd.X1, cmd.Y1);
        var b = _board.GetTile(cmd.X2, cmd.Y2);

        await _anim.SwapAsync(a, b, 0.2f);

        SyncTiles(cmd, a, b);

        _lock.Unlock();
    }

    private void SyncTiles(
        SwapCommand cmd,
        TileView a,
        TileView b)
    {
        _board.SetTile(cmd.X1, cmd.Y1, b);
        _board.SetTile(cmd.X2, cmd.Y2, a);

        a.SetGridPosition(cmd.X2, cmd.Y2);
        b.SetGridPosition(cmd.X1, cmd.Y1);
    }
}