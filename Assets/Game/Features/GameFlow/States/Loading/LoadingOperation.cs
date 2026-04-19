using System;
using System.Numerics;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Loading;
using Game.Application.Loading.Abstractions;

public class MainMenuLoading : ILoadingOperation
{
    public EGameState TargetState => EGameState.MainMenu;

    public async UniTask ExecuteAsync(GameStateContext context, CancellationToken ct)
    {
        var ctx = new LoadingContext(
            context,
            new LoadingProgressAggregator()
        );
        var pipeline = new LoadingPipeline()
            .Use(new MinimumDurationMiddleware(1.5f))
            .Use(new LoadSceneMiddleware("MainMenu"))
            .Use(new LoadAssetsMiddleware( new MainMenuAssetProvider()))
            .Use(new PrewarmMiddleware());

        await pipeline.ExecuteAsync(ctx, ct);
    }
}
public class GamePlayLoading : ILoadingOperation
{
    public EGameState TargetState => EGameState.Gameplay;

    public async UniTask ExecuteAsync(GameStateContext context, CancellationToken ct)
    {
        var ctx = new LoadingContext(
            context,
            new LoadingProgressAggregator()
        );
        var pipeline = new LoadingPipeline()
            .Use(new MinimumDurationMiddleware(1.5f))
            .Use(new LoadSceneMiddleware("Gameplay"))
            .Use(new LoadAssetsMiddleware( new GamePlayAssetProvider()))
            .Use(new PrewarmMiddleware())
            .Use(new SpawnPlayerMiddleware(context.PlayerService));

        await pipeline.ExecuteAsync(ctx, ct);
    }
}
