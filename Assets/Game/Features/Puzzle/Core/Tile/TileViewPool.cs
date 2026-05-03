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
        TileView view;

        if (_pool.Count > 0)
        {
            view = _pool.Pop();
        }
        else
        {
            view = Object.Instantiate(
                _prefab,
                _parent);
        }

        view.gameObject.SetActive(true);

        view.ResetVisual();

        return view;
    }

    public void Release(TileView view)
    {
        view.ResetVisual();

        view.gameObject.SetActive(false);

        _pool.Push(view);
    }
}
