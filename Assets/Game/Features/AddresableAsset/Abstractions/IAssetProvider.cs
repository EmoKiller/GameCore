using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.Android.Gradle.Manifest;
using Game.Application.Core;
using System.Threading;
using System;
using UnityEngine.AddressableAssets;

public interface IAssetProvider : IService
{
    // =====================================================
    // LIFECYCLE
    // =====================================================

    UniTask InitializeAsync(CancellationToken ct);

    // =====================================================
    // LOAD
    // =====================================================

    UniTask<AssetHandle<T>> LoadAsync<T>(string key, CancellationToken ct) where T : UnityEngine.Object;

    UniTask<IAssetHandle> LoadAsync(Type type, string key, CancellationToken ct);

    UniTask<AssetHandle<T>> LoadAsync<T>( AssetReference reference, CancellationToken ct) where T : UnityEngine.Object;

    UniTask<List<AssetHandle<T>>> LoadByLabelAsync<T>(
        string label,
        CancellationToken ct)
        where T : UnityEngine.Object;
    UniTask<List<IAssetHandle>> LoadAllByLabelAsync( string label, CancellationToken ct);
    // =====================================================
    // INSTANTIATE (FIXED)
    // =====================================================

    UniTask<InstanceHandle> InstantiateAsync(
        string key,
        Transform parent, 
        AssetScope scope,
        CancellationToken ct);

    UniTask<InstanceHandle> InstantiateAsync(
        AssetReferenceGameObject reference,
        Transform parent,
        AssetScope scope,
        CancellationToken ct);



    // =====================================================
    // CACHE QUERY
    // =====================================================

    bool TryGetLoaded<T>(string key, out AssetHandle<T> handle)
        where T : UnityEngine.Object;

    // =====================================================
    // PRELOAD
    // =====================================================

    UniTask PreloadAsync(
        IEnumerable<PreloadOperation> operations,
        AssetScope scope,
        CancellationToken ct);

    // =====================================================
    // RELEASE
    // =====================================================
    public UniTask ReleaseInstanceAsync(InstanceHandle handle);
    void Release<T>(AssetHandle<T> handle)
        where T : UnityEngine.Object;

    void ReleaseAll();
    // =====================================================
    // RELEASE (ASYNC )
    // =====================================================

    UniTask ReleaseAsync<T>(AssetHandle<T> handle)
        where T : UnityEngine.Object;
}


public struct PreloadOperation
{
    public string Key;
    public System.Type AssetType;
    public PreloadOperation(string key, System.Type assetType)
    {
        if (string.IsNullOrEmpty(key))
            throw new System.ArgumentException("Key is null or empty");

        Key = key;
        AssetType = assetType ?? throw new System.ArgumentNullException(nameof(assetType));
    }
}