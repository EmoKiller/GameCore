using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class PuzzleBoardAnimator
{
    private PuzzleBoardView _boardView;
    private readonly PuzzleAnimationConfig _config;

    private readonly Dictionary<int, int> _spawnCountsPerColumn = new();

    public PuzzleBoardAnimator(
        PuzzleAnimationConfig config
    )
    {
        _config = config;
    }
    public void InitializeBoard(PuzzleBoardView boardView)
    {
        _boardView = boardView;
    }
    public async UniTask PlayAsync(BoardChangeSet changeSet, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        await PlaySwaps(changeSet, ct);

        ct.ThrowIfCancellationRequested();
        await PlayCreateSpecials(changeSet, ct);

        ct.ThrowIfCancellationRequested();
        await PlayRemoves(changeSet, ct);

        ct.ThrowIfCancellationRequested();
        await PlayFalls(changeSet, ct);

        ct.ThrowIfCancellationRequested();
        await PlaySpawns(changeSet, ct);
    }

    private async UniTask PlaySwaps(BoardChangeSet changeSet, CancellationToken ct)
    {
        var swaps = changeSet.Transitions.OfType<SwapTransition>();
        
        var tasks = swaps.Select(swap => PlaySwapAsync(swap,ct));

        await UniTask.WhenAll(tasks);
    }
    
    private async UniTask PlaySwapAsync(SwapTransition transition, CancellationToken ct)
    {
        TileView fromView = _boardView.GetTileView(transition.From);

        TileView toView = _boardView.GetTileView(transition.To);

        if (fromView == null || toView == null)
        {
            return;
        }

        await AnimateSwapAsync(
            fromView,
            toView,
            transition.From,
            transition.To,
            ct);

        CommitSwapViews(
            transition.From,
            transition.To);
    }
    public async UniTask PlayInvalidSwapAsync(
        TilePosition from,
        TilePosition to,
        CancellationToken ct)
    {
        TileView fromView = _boardView.GetTileView(from);

        TileView toView = _boardView.GetTileView(to);

        if (fromView == null || toView == null)
        {
            return;
        }

        await AnimateSwapAsync(
            fromView,
            toView,
            from,
            to,
            ct);

        await UniTask.Delay(40);

        await AnimateSwapAsync(
            fromView,
            toView,
            to,
            from,
            ct);

        fromView.SetInstantPosition(
            _boardView.Layout.GetWorldPosition(from));

        toView.SetInstantPosition(
            _boardView.Layout.GetWorldPosition(to));
    }
    private void CommitSwapViews( TilePosition from, TilePosition to)
    {
        _boardView.SwapViews(from, to);
    }
    private async UniTask AnimateSwapAsync(
        TileView fromView,
        TileView toView,
        TilePosition from,
        TilePosition to,
        CancellationToken ct)
    {
        Vector3 fromWorld = _boardView.Layout.GetWorldPosition(from);

        Vector3 toWorld = _boardView.Layout.GetWorldPosition(to);

        await UniTask.WhenAll(
            fromView.MoveToAsync(
                toWorld,
                _config.SwapDuration,
                ct
            ),

            toView.MoveToAsync(
                fromWorld,
                _config.SwapDuration,
                ct
            ));
    }

    private async UniTask PlayCreateSpecials(BoardChangeSet changeSet, CancellationToken ct)
    {
        var specials = changeSet.Transitions.OfType<CreateSpecialTransition>();

        foreach (CreateSpecialTransition special in specials)
        {
            PlayCreateSpecial(special);
        }

        await UniTask.CompletedTask;
    }
    private void PlayCreateSpecial(CreateSpecialTransition transition)
    {
        TileView view = _boardView.GetTileView( transition.Position);

        if (view == null)
        {
            return;
        }

        view.SetSpecial(transition.Special);
    }
    private async UniTask PlayRemoves(BoardChangeSet changeSet, CancellationToken ct)
    {
        var removes = changeSet.Transitions.OfType<RemoveTransition>();

        await UniTask.WhenAll(removes.Select(remove => PlayRemoveAsync(remove,ct)));
    }
    private async UniTask PlayRemoveAsync(RemoveTransition transition, CancellationToken ct)
    {
        var view = _boardView.GetTileView(transition.Position);
        if (view == null)
        {
            return;
        }
        await view.ScaleToAsync(
            Vector3.zero,
            _config.RemoveDuration,
            ct
        );

        _boardView.HideView(transition.Position);
    }

    private async UniTask PlayFalls(
        BoardChangeSet changeSet,
        CancellationToken ct
        )
    {
        var falls = changeSet.Transitions.OfType<FallTransition>();

        await UniTask.WhenAll(falls.Select(fall => PlayFallAsync(fall, ct)));
    }
    private async UniTask PlayFallAsync(FallTransition transition, CancellationToken ct)
    {
        var view =
            _boardView.GetTileView(
                transition.From);
        Vector3 target =
            _boardView.Layout.GetWorldPosition(
                transition.To);

        float distance =
            Vector3.Distance(
                view.transform.position,
                target);

        float duration =
            distance *
            _config.FallDurationPerUnit;

        await view.MoveToAsync(
            target,
            duration,
            ct);

        _boardView.MoveView(
            transition.From,
            transition.To);
    }

    private async UniTask PlaySpawns(BoardChangeSet changeSet, CancellationToken ct)
    {
        var spawns = changeSet.Transitions.OfType<SpawnTransition>();

        _spawnCountsPerColumn.Clear();

        await UniTask.WhenAll(spawns.Select(spawn => PlaySpawnAsync(spawn, ct)));
    }

    private async UniTask PlaySpawnAsync(SpawnTransition transition, CancellationToken ct)
    {
        if (_spawnCountsPerColumn.TryGetValue(
                transition.Position.X,
                out int spawnCount) == false)
        {
            spawnCount = 0;
        }

        _spawnCountsPerColumn[
            transition.Position.X] =
                spawnCount + 1;

        var view =
            _boardView.CreateOrReuseView(
                transition.Position,
                transition.TileType);
        Vector3 target =
            _boardView.Layout.GetWorldPosition(
                transition.Position);

        Vector3 spawnPosition =
            _boardView.Layout.GetSpawnWorldPosition(
                transition.Position.X,
                _boardView.Board)

            + Vector3.up *
            (spawnCount *
            _boardView.Layout.TileSize);

        view.transform.localScale =
            Vector3.one;

        view.SetInstantPosition(
            spawnPosition);

        float distance =
            Vector3.Distance(
                spawnPosition,
                target);

        float duration =
            distance *
            _config.FallDurationPerUnit;

        await view.MoveToAsync(
            target,
            duration,
            ct);
    }
}
