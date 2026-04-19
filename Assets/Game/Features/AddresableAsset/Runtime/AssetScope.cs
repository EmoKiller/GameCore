using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AssetScope : IDisposable
{
    private static int _nextId = 1;

    private readonly List<IAssetHandle> _handles = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly object _lock = new();

    private bool _isDisposed;

    public int Id { get; }
    public string Name { get; }

    public CancellationToken Token => _cts.Token;

    public AssetScope(string name = "Scope")
    {
        Id = Interlocked.Increment(ref _nextId);
        Name = name;

#if UNITY_EDITOR
        AssetScopeDebugger.RegisterScope(Id, Name);
#endif
    }

    public void Track(IAssetHandle handle)
    {
        if (handle == null) return;

        if (_isDisposed)
        {
            handle.Release();
            return;
        }

        lock (_lock)
        {
            if (_isDisposed)
            {
                handle.Release();
                return;
            }

            _handles.Add(handle);

#if UNITY_EDITOR
            AssetScopeDebugger.Track(Id, handle, handle.Key);
#endif
        }
    }

    public void Dispose()
    {
        List<IAssetHandle> handlesToRelease;

        lock (_lock)
        {
            if (_isDisposed) return;
            _isDisposed = true;

            handlesToRelease = new List<IAssetHandle>(_handles);
            _handles.Clear();
        }

        _cts.Cancel();

        foreach (var handle in handlesToRelease)
        {
#if UNITY_EDITOR
            AssetScopeDebugger.Untrack(Id, handle);
#endif
            handle.Release();
        }

        _cts.Dispose();

#if UNITY_EDITOR
        AssetScopeDebugger.UnregisterScope(Id);
#endif
    }
}