using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class PuzzleBoardAnimator
{
    private PuzzleBoardView _boardView;
    private readonly PuzzleAnimationConfig _config;
    private readonly SpecialTileVisualDatabase _specialDatabase;

    private readonly Dictionary<int, int> _spawnCountsPerColumn = new();

    public PuzzleBoardAnimator(
        PuzzleAnimationConfig config,
        SpecialTileVisualDatabase specialDatabase
    )
    {
        _config = config;
        _specialDatabase = specialDatabase;
    }
    public void InitializeBoard(PuzzleBoardView boardView)
    {
        _boardView = boardView;
    }
    public async UniTask PlayAsync(
        BoardChangeSet changeSet)
    {
        await PlaySwaps(changeSet);

        await PlayCreateSpecials(changeSet);

        await PlayRemoves(changeSet);

        await PlayFalls(changeSet);

        await PlaySpawns(changeSet);
    }

    private async UniTask PlaySwaps(BoardChangeSet changeSet)
    {
        var swaps = changeSet.Transitions.OfType<SwapTransition>();
        
        var tasks = swaps.Select(PlaySwapAsync);
        await UniTask.WhenAll(tasks);
    }
    private void CommitSwapViews( TilePosition from, TilePosition to)
    {
        _boardView.SwapViews(from, to);
    }
    private async UniTask PlaySwapAsync(SwapTransition transition)
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
            transition.To);

        CommitSwapViews(
            transition.From,
            transition.To);
    }
    public async UniTask PlayInvalidSwapAsync(
        TilePosition from,
        TilePosition to)
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
            to);

        await UniTask.Delay(40);

        await AnimateSwapAsync(
            fromView,
            toView,
            to,
            from);

        fromView.SetInstantPosition(
            _boardView.Layout.GetWorldPosition(from));

        toView.SetInstantPosition(
            _boardView.Layout.GetWorldPosition(to));
    }
    private async UniTask AnimateSwapAsync(
        TileView fromView,
        TileView toView,
        TilePosition from,
        TilePosition to)
    {
        Vector3 fromWorld = _boardView.Layout.GetWorldPosition(from);

        Vector3 toWorld = _boardView.Layout.GetWorldPosition(to);

        await UniTask.WhenAll(
            fromView.MoveToAsync(
                toWorld,
                _config.SwapDuration
            ),

            toView.MoveToAsync(
                fromWorld,
                _config.SwapDuration
            ));
    }

    private async UniTask PlayCreateSpecials(BoardChangeSet changeSet)
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

        view.SetSpecial(transition.Tile, _specialDatabase);
    }
    private async UniTask PlayRemoves(BoardChangeSet changeSet)
    {
        var removes = changeSet.Transitions.OfType<RemoveTransition>();

        await UniTask.WhenAll(removes.Select(PlayRemoveAsync));
    }
    private async UniTask PlayRemoveAsync(RemoveTransition transition)
    {
        var view = _boardView.GetTileView(transition.Position);
        if (view == null)
        {
            return;
        }
        await view.ScaleToAsync(
            Vector3.zero,
            _config.RemoveDuration
        );

        _boardView.HideView(transition.Position);
    }

    private async UniTask PlayFalls(
        BoardChangeSet changeSet)
    {
        var falls = changeSet.Transitions.OfType<FallTransition>();

        await UniTask.WhenAll(falls.Select(PlayFallAsync));
    }
    private async UniTask PlayFallAsync(FallTransition transition)
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
            duration);

        _boardView.MoveView(
            transition.From,
            transition.To);
    }

    private async UniTask PlaySpawns(BoardChangeSet changeSet)
    {
        var spawns = changeSet.Transitions.OfType<SpawnTransition>();

        _spawnCountsPerColumn.Clear();

        await UniTask.WhenAll(spawns.Select(PlaySpawnAsync));
    }

    private async UniTask PlaySpawnAsync(SpawnTransition transition)
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
            duration);
    }
}
