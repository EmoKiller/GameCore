using System;
using System.Collections.Generic;
using UnityEngine;

internal class AssetCacheEntry
{
    public object Handle;
    public int RefCount;
}

public class AssetCache
{
    private readonly Dictionary<(string, Type), AssetCacheEntry> _cache = new();

    // =====================================================
    // GET
    // =====================================================

    public bool TryGet<T>((string, Type) key, out AssetHandle<T> handle)
        where T : UnityEngine.Object
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (entry.Handle is AssetHandle<T> typed)
            {
                entry.RefCount++;
                handle = typed;
                return true;
            }

            throw new InvalidCastException(
                $"[AssetCache] Type mismatch for key: {key.Item1}, expected {typeof(T)}, actual {entry.Handle.GetType()}");
        }

        handle = default;
        return false;
    }

    // =====================================================
    // ADD
    // =====================================================

    public void Add<T>((string, Type) key, AssetHandle<T> handle)
        where T : UnityEngine.Object
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            entry.RefCount++;
            return;
        }

        _cache[key] = new AssetCacheEntry
        {
            Handle = handle,
            RefCount = 1
        };
    }

    // =====================================================
    // RELEASE
    // =====================================================

    public bool Release<T>((string, Type) key, out AssetHandle<T> handle)
        where T : UnityEngine.Object
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            entry.RefCount--;

            if (entry.RefCount <= 0)
            {
                if (entry.Handle is AssetHandle<T> typed)
                {
                    handle = typed;
                    _cache.Remove(key);
                    return true;
                }

                throw new InvalidCastException(
                    $"[AssetCache] Type mismatch on release: {key.Item1}");
            }
        }

        handle = default;
        return false;
    }

    // =====================================================
    // ENUM
    // =====================================================

    public IEnumerable<object> GetAllHandles()
    {
        foreach (var entry in _cache.Values)
        {
            yield return entry.Handle;
        }
    }

    // =====================================================
    // CLEAR
    // =====================================================

    public void Clear()
    {
        _cache.Clear();
    }
}