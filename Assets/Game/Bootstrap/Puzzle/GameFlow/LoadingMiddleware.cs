using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core.SceneLoader;
using Game.Application.Events;
using Game.Presentation.UI.View;
using UnityEngine;



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
    private readonly IEventBus _eventbus;
    private readonly ISceneLoader _sceneLoader;

    public LoadSceneMiddleware( string sceneName, IEventBus eventbus , ISceneLoader sceneLoader)
    {
        _sceneName = sceneName;
        _eventbus = eventbus;
        _sceneLoader = sceneLoader;
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
            _eventbus.Publish(new LoadingProgressEvent(ctx.Progress.GetTotalProgress()),EventChannel.UI);
        });

        await _sceneLoader.LoadScene(_sceneName, progress, ct);

        await next();
    }
}
public sealed class LoadAssetsMiddleware : IWeightedLoadingMiddleware
{
    private readonly ILoadingAssetProvider _LoadingAssetProvider;
    private readonly IEventBus _eventbus;
    private readonly IAssetProvider _assetProvider;

    public LoadAssetsMiddleware(ILoadingAssetProvider loadingAssetProvider , IEventBus eventbus, IAssetProvider assetProvider)
    {
        _LoadingAssetProvider = loadingAssetProvider;
        _eventbus = eventbus;
        _assetProvider = assetProvider;
    }

    public float Weight => 0.3f;

    public async UniTask InvokeAsync(
        LoadingContext ctx,
        Func<UniTask> next,
        CancellationToken ct)
    {
        var labels = _LoadingAssetProvider.GetAssetLabels();

        for (int i = 0; i < labels.Count; i++)
        {
            await _assetProvider.LoadAllByLabelAsync(labels[i], ct);

            ctx.Progress.Report(1, (i + 1) / (float)labels.Count);
            _eventbus.Publish(new LoadingProgressEvent(ctx.Progress.GetTotalProgress()),EventChannel.UI);
        }

        await next();
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