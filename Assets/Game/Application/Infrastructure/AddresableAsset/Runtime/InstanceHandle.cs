using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class InstanceHandle : IAssetHandle
{
    public string Key { get; }
    public GameObject Instance { get; }
    public AsyncOperationHandle<GameObject> Handle { get; }

    private bool _isReleased;

    public InstanceHandle(string key, GameObject instance, AsyncOperationHandle<GameObject> handle)
    {
        Key = key;
        Instance = instance;
        Handle = handle;
    }

    public void Release()
    {
        if (_isReleased) return;
        _isReleased = true;

        if (Handle.IsValid())
        {
            Addressables.ReleaseInstance(Instance);
        }
    }

    public void ReleaseDirect()
    {
        Release();
    }
}