using System.Collections.Generic;
using UnityEngine;

public sealed class TileViewPool
{
    private readonly TileView _prefab;

    private readonly Transform _parent;

    private readonly Stack<TileView> _pool =
        new();

    public TileViewPool(
        TileView prefab,
        Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }

    public TileView Get()
    {
        if (_pool.Count > 0)
        {
            TileView view =
                _pool.Pop();
            
            view.gameObject.SetActive(true);

            return view;
        }

        return Object.Instantiate(
            _prefab,
            _parent);
    }

    public void Release(
        TileView view)
    {
        view.gameObject.SetActive(false);

        _pool.Push(view);
    }
}
