using UnityEngine;

public interface IAssetHandle
{
    string Key { get; }
    void Release();
    void ReleaseDirect();
}
