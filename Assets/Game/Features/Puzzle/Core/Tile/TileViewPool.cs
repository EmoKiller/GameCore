using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public sealed class TileViewPool : IObjectPool<TileView> , IDisposable
{
    private readonly TileView _prefab;

    private readonly Transform _parent;

    //private readonly Stack<TileView> _pool = new();
    private ObjectPool<TileView> _pool;
    public int CountInactive => _pool.CountInactive;

    public TileViewPool(
        TileView prefab,
        Transform parent)
    {
        _prefab = prefab;
        _parent = parent;

        _pool = new ObjectPool<TileView>(
            Create,
            OnGet,
            OnRelease,
            OnDestroyPoolObject,
            collectionCheck: true,
            defaultCapacity: 16,
            maxSize: 128
        );
    }
    public TileView Get()
    {
        return _pool.Get();
    }

    public PooledObject<TileView> Get(out TileView value)
    {
        return _pool.Get(out value);
    }

    public void Release(TileView element)
    {
        if (element == null)
        {
            return;
        }

        _pool.Release(element);
    }

    private TileView Create()
    {
        TileView view = UnityEngine.Object.Instantiate(
            _prefab,
            _parent);

        view.gameObject.SetActive(false);

        return view;
    }

    private void OnGet(TileView view)
    {
        view.gameObject.SetActive(true);

        view.ResetVisual();
    }

    private void OnRelease(TileView view)
    {
        view.ResetVisual();

        view.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(TileView view)
    {
        if (view != null)
        {
            UnityEngine.Object.Destroy(view.gameObject);
        }
    }
    public void Clear()
    {
        _pool.Clear();
    }
    public void Dispose()
    {
        _pool.Dispose();
    }
    // public TileView Get()
    // {
    //     TileView view;

    //     if (_pool.Count > 0)
    //     {
    //         view = _pool.Pop();
    //     }
    //     else
    //     {
    //         view = Object.Instantiate(
    //             _prefab,
    //             _parent);
    //     }

    //     view.gameObject.SetActive(true);

    //     view.ResetVisual();

    //     return view;
    // }

    // public void Release(TileView view)
    // {
    //     view.ResetVisual();

    //     view.gameObject.SetActive(false);

    //     _pool.Push(view);
    // }
}
