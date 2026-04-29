using System;
using System.Numerics;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Loading;
using Game.Application.Loading.Abstractions;

public class MainMenuLoading : ILoadingOperation<RPGGameStateContext>
{
    public EGameState TargetState => EGameState.MainMenu;

    public async UniTask ExecuteAsync(RPGGameStateContext context, CancellationToken ct)
    {
        var ctx = new LoadingContext(
            new LoadingProgressAggregator()
        );
        var pipeline = new LoadingPipeline()
            .Use(new MinimumDurationMiddleware(1.5f))
            .Use(new LoadSceneMiddleware("MainMenu", context.EventBus, context.SceneLoader))
            .Use(new LoadAssetsMiddleware( new MainMenuAssetProvider(), context.EventBus, context.AssetProvider))
            .Use(new PrewarmMiddleware());

        await pipeline.ExecuteAsync(ctx, ct);
    }
}
public class GamePlayLoading : ILoadingOperation<RPGGameStateContext>
{
    public EGameState TargetState => EGameState.Gameplay;

    public async UniTask ExecuteAsync(RPGGameStateContext context, CancellationToken ct)
    {
        var ctx = new LoadingContext(
            new LoadingProgressAggregator()
        );
        var pipeline = new LoadingPipeline()
            .Use(new MinimumDurationMiddleware(1.5f))
            .Use(new LoadSceneMiddleware("Gameplay", context.EventBus, context.SceneLoader))
            .Use(new LoadAssetsMiddleware( new GamePlayAssetProvider(), context.EventBus, context.AssetProvider))
            .Use(new PrewarmMiddleware())
            .Use(new SpawnPlayerMiddleware(context.PlayerService));

        await pipeline.ExecuteAsync(ctx, ct);
    }
}
