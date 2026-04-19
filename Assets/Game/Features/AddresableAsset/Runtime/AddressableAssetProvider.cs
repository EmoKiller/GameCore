using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Collections.Generic;
using System.Threading;


public class AddressableAssetProvider : IAssetProvider
{
    private readonly AssetCache _cache = new();

    private readonly Dictionary<(string, Type), UniTaskCompletionSource<IAssetHandle>> _loadingTasks = new();
    private readonly object _lock = new();
    private readonly object _dynamicLoaderLock = new();
    private readonly Dictionary<Type, Func<string, CancellationToken, UniTask<IAssetHandle>>> _dynamicLoaders = new();


    // =====================================================
    // LIFECYCLE
    // =====================================================
    
    public async UniTask InitializeAsync(CancellationToken ct)
    {
        await Addressables.InitializeAsync().ToUniTask(cancellationToken: ct);
    }
    
    // =====================================================
    // LOAD SINGLE
    // =====================================================

    public async UniTask<AssetHandle<T>> LoadAsync<T>(string key, CancellationToken ct)
    where T : UnityEngine.Object
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key is null or empty", nameof(key));

        var cacheKey = (key, typeof(T));

        if (_cache.TryGet(cacheKey, out AssetHandle<T> cached))
            return cached;

        UniTaskCompletionSource<IAssetHandle> tcs;

        lock (_lock)
        {
            if (_cache.TryGet(cacheKey, out cached))
                return cached;

            if (!_loadingTasks.TryGetValue(cacheKey, out tcs))
            {
                tcs = new UniTaskCompletionSource<IAssetHandle>();
                _loadingTasks[cacheKey] = tcs;

                // 🔥 truyền ct vào
                RunLoadTask<T>(key, cacheKey, tcs, ct).Forget();
            }
        }

        var handle = await tcs.Task.AttachExternalCancellation(ct);
        var result = (AssetHandle<T>)handle;

        // đảm bảo luôn trả về cache nếu có
        if (_cache.TryGet(cacheKey, out AssetHandle<T> latest))
            return latest;

        return result;
    }
    private async UniTaskVoid RunLoadTask<T>(
    string key,
    (string, Type) cacheKey,
    UniTaskCompletionSource<IAssetHandle> tcs,
    CancellationToken ct)
    where T : UnityEngine.Object
    {
        try
        {
            var result = await LoadInternal<T>(key, cacheKey, ct);

            // 🔥 Guard: nếu ReleaseAll đã clear _loadingTasks
            lock (_lock)
            {
                if (!_loadingTasks.TryGetValue(cacheKey, out var current) ||
                    !ReferenceEquals(current, tcs))
                {
                    // Task này không còn hợp lệ nữa → release để tránh leak
                    if (result.Handle.IsValid())
                        Addressables.Release(result.Handle);

                    return;
                }
            }

            if (ct.IsCancellationRequested)
            {
                result.ReleaseDirect(); // 🔥 QUAN TRỌNG
                tcs.TrySetCanceled();
                return;
            }

            tcs.TrySetResult(result);
        }
        catch (OperationCanceledException)
        {
            tcs.TrySetCanceled();
        }
        catch (Exception ex)
        {
            tcs.TrySetException(ex);
        }
        finally
        {
            lock (_lock)
            {
                if (_loadingTasks.TryGetValue(cacheKey, out var current) &&
                    ReferenceEquals(current, tcs))
                {
                    _loadingTasks.Remove(cacheKey);
                }
            }
        }
    }

    private async UniTask<AssetHandle<T>> LoadInternal<T>(
    string key,
    (string, Type) cacheKey,
    CancellationToken ct)
    where T : UnityEngine.Object
    {
        var handle = Addressables.LoadAssetAsync<T>(key);

        T resultObj;
        try
        {
            resultObj = await AwaitHandle(handle, ct);
        }
        catch
        {
            if (handle.IsValid())
                Addressables.Release(handle);
            throw;
        }

        var result = new AssetHandle<T>(key, resultObj, handle, this);

        if (ct.IsCancellationRequested)
        {
            if (handle.IsValid())
                Addressables.Release(handle);

            throw new OperationCanceledException();
        }

        lock (_lock)
        {
            if (!_cache.TryGet(cacheKey, out AssetHandle<T> existing))
            {
                _cache.Add(cacheKey, result);
            }
            else
            {
                if (handle.IsValid())
                    Addressables.Release(handle);

                return existing;
            }
        }

        return result;
    }
    
    public async UniTask<List<IAssetHandle>> LoadAllByLabelAsync(
    string label,
    CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new ArgumentException("Label is null or empty", nameof(label));

        ct.ThrowIfCancellationRequested();

        var locationsHandle = Addressables.LoadResourceLocationsAsync(label);

        IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation> locations;

        try
        {
            locations = await AwaitHandle(locationsHandle, ct);
        }
        finally
        {
            if (locationsHandle.IsValid())
                Addressables.Release(locationsHandle);
        }

        if (locations == null || locations.Count == 0)
            return new List<IAssetHandle>();

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

        var tasks = new List<UniTask<IAssetHandle>>(locations.Count);

        foreach (var loc in locations)
        {
            linkedCts.Token.ThrowIfCancellationRequested();

            tasks.Add(LoadFromLocation(loc, linkedCts.Token));
        }

        try
        {
            var results = await UniTask.WhenAll(tasks);
            return new List<IAssetHandle>(results);
        }
        catch
        {
            linkedCts.Cancel();
            throw;
        }
    }
    private async UniTask<IAssetHandle> LoadFromLocation(
    UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation location,
    CancellationToken ct)
    {
        var handle = Addressables.LoadAssetAsync<UnityEngine.Object>(location);

        UnityEngine.Object obj;

        try
        {
            obj = await AwaitHandle(handle, ct);
        }
        catch
        {
            if (handle.IsValid())
                Addressables.Release(handle);
            throw;
        }

        // ⚠️ KHÔNG add vào cache (tránh conflict type)
        var assetHandle = new AssetHandle<UnityEngine.Object>(
            location.PrimaryKey,
            obj,
            handle,
            this);

        return assetHandle;
    }
    
    // =====================================================
    // DYNAMIC LOAD
    // =====================================================

    public UniTask<IAssetHandle> LoadAsync(Type type, string key, CancellationToken ct)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key is null or empty", nameof(key));

        Func<string, CancellationToken, UniTask<IAssetHandle>> loader;

        lock (_dynamicLoaderLock)
        {
            if (!_dynamicLoaders.TryGetValue(type, out loader))
            {
                loader = CreateDynamicLoader(type);
                _dynamicLoaders[type] = loader;
            }
        }

        return loader(key, ct);
    }

    private Func<string, CancellationToken, UniTask<IAssetHandle>> CreateDynamicLoader(Type type)
    {
        var method = typeof(AddressableAssetProvider)
            .GetMethod(nameof(LoadGeneric), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (method == null)
            throw new InvalidOperationException("LoadGeneric method not found.");

        var genericMethod = method.MakeGenericMethod(type);

        return (Func<string, CancellationToken, UniTask<IAssetHandle>>)
            Delegate.CreateDelegate(
                typeof(Func<string, CancellationToken, UniTask<IAssetHandle>>),
                this,
                genericMethod);
    }

    private async UniTask<IAssetHandle> LoadGeneric<T>(string key, CancellationToken ct)
        where T : UnityEngine.Object
    {
        return await LoadAsync<T>(key, ct);
    }

    // =====================================================
    // LOAD BY LABEL
    // =====================================================

    public async UniTask<List<AssetHandle<T>>> LoadByLabelAsync<T>(
    string label,
    CancellationToken ct)
    where T : UnityEngine.Object
    {
        ct.ThrowIfCancellationRequested();

        var locationsHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));

        IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation> locations;
        try
        {
            locations = await AwaitHandle(locationsHandle, ct);
        }
        finally
        {
            if (locationsHandle.IsValid())
                Addressables.Release(locationsHandle);
        }

        if (locations == null || locations.Count == 0)
            return new List<AssetHandle<T>>();

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

        var tasks = new List<UniTask<AssetHandle<T>>>(locations.Count);

        foreach (var loc in locations)
        {
            linkedCts.Token.ThrowIfCancellationRequested();
            tasks.Add(LoadAsync<T>(loc.PrimaryKey, linkedCts.Token));
        }

        try
        {
            var results = await UniTask.WhenAll(tasks);
            return new List<AssetHandle<T>>(results);
        }
        catch
        {
            linkedCts.Cancel(); // 🔥 cancel các task còn lại
            throw;
        }
    }

    // =====================================================
    // INSTANTIATE
    // =====================================================

    public async UniTask<InstanceHandle> InstantiateAsync(
    string key,
    Transform parent,
    AssetScope scope,
    CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        CancellationToken finalToken;
        CancellationTokenSource linkedCts = null;

        if (scope != null)
        {
            linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, scope.Token);
            finalToken = linkedCts.Token;
        }
        else
        {
            finalToken = ct;
        }

        var handle = Addressables.InstantiateAsync(key, parent);

        GameObject instance;
        try
        {
            instance = await AwaitHandle(handle, finalToken);
        }
        catch
        {
            if (handle.IsValid())
                Addressables.Release(handle);

            throw;
        }
        finally
        {
            linkedCts?.Dispose();
        }

        if (finalToken.IsCancellationRequested)
        {
            if (handle.IsValid())
                Addressables.Release(handle);

            throw new OperationCanceledException();
        }

        var instanceHandle = new InstanceHandle(key, instance, handle);

        scope?.Track(instanceHandle);

        return instanceHandle;
    }

    // =====================================================
    // CACHE QUERY
    // =====================================================

    public bool TryGetLoaded<T>(string key, out AssetHandle<T> handle)
    where T : UnityEngine.Object
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            handle = default;
            return false;
        }

        return _cache.TryGet((key, typeof(T)), out handle);
    }

    // =====================================================
    // PRELOAD
    // =====================================================

    public async UniTask PreloadAsync(
    IEnumerable<PreloadOperation> operations,
    AssetScope scope,
    CancellationToken ct)
    {
        if (operations == null)
            return;

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

        var tasks = new List<UniTask<IAssetHandle>>(
            operations is ICollection<PreloadOperation> col ? col.Count : 8);

        foreach (var op in operations)
        {
            linkedCts.Token.ThrowIfCancellationRequested();
            tasks.Add(LoadAsync(op.AssetType, op.Key, linkedCts.Token));
        }

        IAssetHandle[] handles;

        try
        {
            handles = await UniTask.WhenAll(tasks);
        }
        catch
        {
            linkedCts.Cancel(); // 🔥 cancel các task còn lại
            throw;
        }

        if (scope != null)
        {
            foreach (var h in handles)
                scope.Track(h);
        }
    }

    // =====================================================
    // RELEASE
    // =====================================================

    public void Release<T>(AssetHandle<T> handle)
    where T : UnityEngine.Object
    {
        if (handle.Key == null)
            return;

        if (!handle.Handle.IsValid())
            return;

        var key = (handle.Key, typeof(T));

        if (_cache.Release(key, out AssetHandle<T> cached))
        {
            if (cached.Handle.IsValid())
            {
                Addressables.Release(cached.Handle);
            }
        }
    }

    public void ReleaseAll()
    {
        lock (_lock)
        {
            foreach (var obj in _cache.GetAllHandles())
            {
                if (obj is IAssetHandle handle)
                {
                    handle.ReleaseDirect();
                }
            }

            _cache.Clear();
            _loadingTasks.Clear();
        }

        lock (_dynamicLoaderLock)
        {
            _dynamicLoaders.Clear();
        }
    }
    // =====================================================
    // UTIL
    // =====================================================

    private async UniTask<T> AwaitHandle<T>(
    AsyncOperationHandle<T> handle,
    CancellationToken ct,
    bool releaseOnFailure = true)
    {
        try
        {
            // 🚨 KHÔNG truyền ct vào đây
            await handle.ToUniTask(cancellationToken: CancellationToken.None);
        }
        catch
        {
            // ✔ CHỈ release nếu operation FAILED thật
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                if (releaseOnFailure && handle.IsValid())
                    Addressables.Release(handle);
            }

            throw;
        }

        // ✔ cancellation xảy ra sau await → SAFE
        ct.ThrowIfCancellationRequested();

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            if (releaseOnFailure && handle.IsValid())
                Addressables.Release(handle);

            throw new Exception(
                $"[Addressables] Operation failed: {handle.DebugName}, Status: {handle.Status}");
        }

        return handle.Result;
    }
    // =====================================================
    // AssetReference
    // =====================================================


    public UniTask<AssetHandle<T>> LoadAsync<T>(
    AssetReference reference,
    CancellationToken ct)
    where T : UnityEngine.Object
    {
        if (reference == null)
            throw new ArgumentNullException(nameof(reference));

        var key = reference.RuntimeKey.ToString();

        // reuse toàn bộ pipeline cũ
        return LoadAsync<T>(key, ct);
    }
    public async UniTask<InstanceHandle> InstantiateAsync(
        AssetReferenceGameObject reference,
        Transform parent,
        AssetScope scope,
        CancellationToken ct)
    {
        if (reference == null)
            throw new ArgumentNullException(nameof(reference));

        ct.ThrowIfCancellationRequested();

        CancellationToken finalToken;
        CancellationTokenSource linkedCts = null;

        if (scope != null)
        {
            linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, scope.Token);
            finalToken = linkedCts.Token;
        }
        else
        {
            finalToken = ct;
        }

        var handle = reference.InstantiateAsync(parent);

        GameObject instance;
        try
        {
            instance = await AwaitHandle(handle, finalToken);
        }
        catch
        {
            if (handle.IsValid())
                Addressables.Release(handle);

            throw;
        }
        finally
        {
            linkedCts?.Dispose();
        }

        if (finalToken.IsCancellationRequested)
        {
            if (handle.IsValid())
                Addressables.Release(handle);

            throw new OperationCanceledException();
        }

        var instanceHandle = new InstanceHandle(
            reference.RuntimeKey.ToString(),
            instance,
            handle);

        scope?.Track(instanceHandle);

        return instanceHandle;
    }
    public async UniTask<T> InstantiateAsync<T>(
        AssetReferenceGameObject reference,
        Transform parent,
        AssetScope scope,
        CancellationToken ct)
        where T : Component
    {
        var handle = await InstantiateAsync(reference, parent, scope, ct);

        var component = handle.Instance.GetComponent<T>();

        if (component == null)
            throw new Exception($"Component {typeof(T)} not found on prefab: {reference.RuntimeKey}");

        return component;
    }

    public UniTask ReleaseAsync<T>(AssetHandle<T> handle)
        where T : UnityEngine.Object
    {
        if (handle.Key == null)
            return UniTask.CompletedTask;

        if (!handle.Handle.IsValid())
            return UniTask.CompletedTask;

        var key = (handle.Key, typeof(T));

        if (_cache.Release(key, out AssetHandle<T> cached))
        {
            if (cached.Handle.IsValid())
            {
                Addressables.Release(cached.Handle);
            }
        }

        return UniTask.CompletedTask;
    }
    public UniTask ReleaseAsync(IAssetHandle handle)
    {
        if (handle == null)
            return UniTask.CompletedTask;

        handle.ReleaseDirect(); // dùng abstraction của bạn

        return UniTask.CompletedTask;
    }
    public UniTask ReleaseInstanceAsync(InstanceHandle handle)
    {
        if (handle.Instance == null)
            return UniTask.CompletedTask;

        if (handle.Handle.IsValid())
        {
            Addressables.ReleaseInstance(handle.Handle);
        }
        else
        {
            UnityEngine.Object.Destroy(handle.Instance);
        }

        return UniTask.CompletedTask;
    }
    
}