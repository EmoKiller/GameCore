using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class PuzzleBoardAnimator
{
    private readonly PuzzleBoardView _boardView;

    public PuzzleBoardAnimator(PuzzleBoardView boardView)
    {
        _boardView = boardView;
    }

    public async UniTask PlayAsync(
        BoardChangeSet changeSet)
    {
        await PlaySwaps(changeSet);

        await PlayRemoves(changeSet);

        await PlayFalls(changeSet);

        await PlaySpawns(changeSet);
    }

    private async UniTask PlaySwaps(
        BoardChangeSet changeSet)
    {
        var swaps = changeSet.Transitions.OfType<SwapTransition>();
        // List<UniTask> tasks = new List<UniTask>();
        // foreach (SwapTransition swap in swaps)
        // {
        //     tasks.Add(PlaySwapAsync(swap));

        // }
        var tasks = swaps.Select(PlaySwapAsync);
        await UniTask.WhenAll(tasks);
    }

    private async UniTask PlayRemoves(
        BoardChangeSet changeSet)
    {
        var removes =
            changeSet.Transitions
                .OfType<RemoveTransition>();

        await UniTask.WhenAll(
            removes.Select(PlayRemoveAsync));
    }

    private async UniTask PlayFalls(
        BoardChangeSet changeSet)
    {
        var falls =
            changeSet.Transitions
                .OfType<FallTransition>();

        await UniTask.WhenAll(
            falls.Select(PlayFallAsync));
    }

    private async UniTask PlaySpawns(
        BoardChangeSet changeSet)
    {
        var spawns =
            changeSet.Transitions
                .OfType<SpawnTransition>();

        await UniTask.WhenAll(
            spawns.Select(PlaySpawnAsync));
    }

    private async UniTask PlaySwapAsync(
        SwapTransition transition)
    {
        TileView fromView =
            _boardView.GetTileView(
                transition.From);

        TileView toView =
            _boardView.GetTileView(
                transition.To);
        if (fromView == null || toView == null)
        {
            return;
        }
        Vector3 fromWorld =
            _boardView.Layout.GetWorldPosition(
                transition.From);        

        Vector3 toWorld =
            _boardView.Layout.GetWorldPosition(
                transition.To);

        

        await UniTask.WhenAll(
            fromView.MoveToAsync(
                toWorld,
                0.15f),

            toView.MoveToAsync(
                fromWorld,
                0.15f));
        
        _boardView.SwapViews(
            transition.From,
            transition.To);
    }

    private async UniTask PlayRemoveAsync(
        RemoveTransition transition)
    {
        var view =
            _boardView.GetTileView(
                transition.Position);

        await view.ScaleToAsync(
            Vector3.zero,
            0.15f);

        _boardView.HideView(
            transition.Position);
    }

    private async UniTask PlayFallAsync(
        FallTransition transition)
    {
        var view =
            _boardView.GetTileView(
                transition.From);

        Vector3 target =
            _boardView.Layout.GetWorldPosition(
                transition.To);

        await view.MoveToAsync(
            target,
            0.2f);

        _boardView.MoveView(
            transition.From,
            transition.To);
    }

    private async UniTask PlaySpawnAsync(
        SpawnTransition transition)
    {
        var view =
            _boardView.CreateOrReuseView(
                transition.Position,
                transition.TileType);

        Vector3 target =
            _boardView.Layout.GetWorldPosition(
                transition.Position);

        Vector3 spawnPosition =
            target + Vector3.up * 2f;

        view.SetInstantPosition(
            spawnPosition);

        view.transform.localScale =
            Vector3.zero;

        await UniTask.WhenAll(
            view.MoveToAsync(
                target,
                0.2f),

            view.ScaleToAsync(
                Vector3.one,
                0.15f));
    }
}
