using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class AssetHandle<T> : IAssetHandle
    where T : Object
{
    public string Key { get; }
    public T Asset { get; }
    public AsyncOperationHandle<T> Handle { get; }

    private readonly IAssetProvider _provider;
    private bool _isReleased;

    public AssetHandle(string key, T asset, AsyncOperationHandle<T> handle, IAssetProvider provider)
    {
        Key = key;
        Asset = asset;
        Handle = handle;
        _provider = provider;
    }

    public void Release()
    {
        if (_isReleased) return;
        _isReleased = true;

        Debug.Log($"[AssetHandle] Releasing: {Key}");
        
        _provider.Release(this);
    }

    public void ReleaseDirect()
    {
        if (_isReleased) return;
        _isReleased = true;

        if (Handle.IsValid())
        {
            Addressables.Release(Handle);
        }
    }
}
