using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Loading.Abstractions;
using UnityEngine;

public class PuzzleMainMenuLoading : ILoadingOperation<PuzzleGameStateContext>
{
    public EGameState TargetState => EGameState.MainMenu;

    public async UniTask ExecuteAsync(PuzzleGameStateContext context, CancellationToken ct)
    {
        var (bus, loader, assets) = (context.EventBus, context.SceneLoader, context.AssetProvider);
        var ctx = new LoadingContext(
            new LoadingProgressAggregator()
        );
        var pipeline = new LoadingPipeline()
            .Use(new MinimumDurationMiddleware(0.5f))
            .Use(new LoadSceneMiddleware("MainMenu", bus, loader))
            .Use(new LoadAssetsMiddleware( new MainMenuAssetProvider(), bus, assets))
            .Use(new PrewarmMiddleware());

        await pipeline.ExecuteAsync(ctx, ct);
    }
}
public class PuzzleGamePlayLoading : ILoadingOperation<PuzzleGameStateContext>
{
    public EGameState TargetState => EGameState.Gameplay;

    public async UniTask ExecuteAsync(PuzzleGameStateContext context, CancellationToken ct)
    {
        var (bus, loader, assets) = (context.EventBus, context.SceneLoader, context.AssetProvider);
        var ctx = new LoadingContext(
            new LoadingProgressAggregator()
        );
        var pipeline = new LoadingPipeline()
            .Use(new MinimumDurationMiddleware(0.5f))
            .Use(new LoadSceneMiddleware("Gameplay", bus, loader))
            .Use(new LoadAssetsMiddleware( new GamePlayAssetProvider(), bus, assets))
            .Use(new PrewarmMiddleware());

        await pipeline.ExecuteAsync(ctx, ct);
    }
}
