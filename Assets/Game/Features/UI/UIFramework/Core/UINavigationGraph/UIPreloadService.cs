using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
public interface IUIPreloadService
{
    UniTask PreloadAsync(Type viewType, CancellationToken ct);
}
public sealed class UIPreloadService : IUIPreloadService
{
    private readonly UIManifest _manifest;
    private readonly IAssetProvider _asset;

    public UIPreloadService(UIManifest manifest, IAssetProvider asset)
    {
        _manifest = manifest;
        _asset = asset;
    }

    public async UniTask PreloadAsync(Type viewType, CancellationToken ct)
    {
        var entry = _manifest.Get(viewType);

        await _asset.LoadAsync<UnityEngine.GameObject>(
            entry.AssetKey,
            ct);
    }
}
