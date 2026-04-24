using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Events;
using Game.Presentation.UI.View;
using UnityEngine;

public interface ILoadingMiddleware
{
    UniTask InvokeAsync(LoadingContext ctx, Func<UniTask> next, CancellationToken ct);
}

public interface IWeightedLoadingMiddleware : ILoadingMiddleware
{
    float Weight { get; }
}
public sealed class MinimumDurationMiddleware : ILoadingMiddleware, IWeightedLoadingMiddleware
{
    public float Weight => 0.01f; // rất nhỏ, gần như không ảnh hưởng progress

    private readonly float _minDuration;

    public MinimumDurationMiddleware(float minDurationSeconds = 1.0f)
    {
        _minDuration = minDurationSeconds;
    }

    public async UniTask InvokeAsync(
        LoadingContext ctx,
        Func<UniTask> next,
        CancellationToken ct)
    {
        float startTime = Time.realtimeSinceStartup;

        await next(); // chạy toàn bộ pipeline phía sau trước

        float elapsed = Time.realtimeSinceStartup - startTime;
        float remaining = _minDuration - elapsed;

        if (remaining > 0f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(remaining), cancellationToken: ct);
        }
    }
}
public sealed class LoadSceneMiddleware : IWeightedLoadingMiddleware
{
    private readonly string _sceneName;

    public LoadSceneMiddleware( string sceneName)
    {
        _sceneName = sceneName;
    }

    public float Weight => 0.7f;

    public async UniTask InvokeAsync(
        LoadingContext ctx,
        Func<UniTask> next,
        CancellationToken ct)
    {
        var progress = new Progress<float>(p =>
        {
            ctx.Progress.Report(0, p);

            //ctx.TotalProgress.Value = ctx.Progress.GetTotalProgress();
            ctx.Game.EventBus.Publish(new LoadingProgressEvent(ctx.Progress.GetTotalProgress()),EventChannel.UI);
        });

        await ctx.Game.SceneLoader.LoadScene(_sceneName, progress, ct);

        await next();
    }
}
public sealed class LoadAssetsMiddleware : IWeightedLoadingMiddleware
{
    private readonly ILoadingAssetProvider _assetProvider;

    public LoadAssetsMiddleware(ILoadingAssetProvider assetProvider)
    {
        _assetProvider = assetProvider;
    }

    public float Weight => 0.3f;

    public async UniTask InvokeAsync(
        LoadingContext ctx,
        Func<UniTask> next,
        CancellationToken ct)
    {
        var labels = _assetProvider.GetAssetLabels();

        for (int i = 0; i < labels.Count; i++)
        {
            await ctx.Game.AssetProvider.LoadAllByLabelAsync(labels[i], ct);

            ctx.Progress.Report(1, (i + 1) / (float)labels.Count);
            //ctx.TotalProgress.Value = ctx.Progress.GetTotalProgress();
            ctx.Game.EventBus.Publish(new LoadingProgressEvent(ctx.Progress.GetTotalProgress()),EventChannel.UI);
        }

        await next();
    }
}
public sealed class SpawnPlayerMiddleware : ILoadingMiddleware
{
    private readonly IPlayerService _playerService;
    //private readonly Transform _parent;

    public SpawnPlayerMiddleware(IPlayerService playerService)//, Transform parent)
    {
        _playerService = playerService;
       // _parent = parent;
    }

    public async UniTask InvokeAsync(
        LoadingContext context,
        Func<UniTask> next,
        CancellationToken ct)
    {
        await next();

        // đảm bảo scene đã load xong
        _playerService.Spawn();
    }
}
public sealed class PrewarmMiddleware : IWeightedLoadingMiddleware
{
    public float Weight => 0.1f;

    public async UniTask InvokeAsync(
        LoadingContext ctx,
        Func<UniTask> next,
        CancellationToken ct)
    {
        // preload shaders, pools, config
        await UniTask.Yield(ct);

        await next();
    }
}