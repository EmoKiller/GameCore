using UnityEngine;

public sealed class TileLayer
{
    private readonly TileViewPool _pool;

    private readonly TileVisualDatabase _visualDatabase;

    private readonly IReadOnlyBoardLayout _layout;

    private TileView[,] _tileViews;

    public TileLayer(
        TileView prefab,
        Transform parent,
        IReadOnlyBoardLayout layout,
        TileVisualDatabase visualDatabase)
    {
        _layout = layout;

        _visualDatabase = visualDatabase;

        _pool = new TileViewPool( prefab, parent);
    }

    public void Initialize(IReadOnlyPuzzleBoard board)
    {
        _tileViews = new TileView[board.Width,board.Height];
    }

    public void Create(int x, int y)
    {
        TileView view = _pool.Get();

        TilePosition position =
            new TilePosition(x, y);

        view.SetPosition(position);

        view.SetWorldPosition(
            _layout.GetWorldPosition(
                position));

        view.gameObject.name = $"Tile ({x}, {y})";

        _tileViews[x, y] = view;
    }

    public void Refresh(IReadOnlyPuzzleBoard board, int x, int y)
    {
        TileData tile = board.Get(x, y);

        TileView view = _tileViews[x, y];

        Sprite sprite = _visualDatabase.GetSprite(tile.Type);

        view.SetSprite(sprite);
    }

    public void Move(TilePosition from, TilePosition to)
    {
        TileView view = Get(from);

        _tileViews[to.X, to.Y] = view;

        _tileViews[from.X, from.Y] = null;

        if (view != null)
        {
            view.SetPosition(to);
        }
    }

    public void Swap(TilePosition a, TilePosition b)
    {
        TileView viewA = Get(a);

        TileView viewB = Get(b);

        _tileViews[a.X, a.Y] = viewB;

        _tileViews[b.X, b.Y] = viewA;

        if (viewA != null)
        {
            viewA.SetPosition(b);
        }

        if (viewB != null)
        {
            viewB.SetPosition(a);
        }
    }

    public void Hide(TilePosition position)
    {
        TileView view = Get(position);

        if (view == null)
        {
            return;
        }

        _tileViews[position.X, position.Y] = null;

        _pool.Release(view);
    }

    public TileView CreateOrReuse(TilePosition position, ETileType tileType)
    {
        TileView view = _pool.Get();

        view.SetPosition(position);

        view.SetSprite(_visualDatabase.GetSprite(tileType));

        _tileViews[position.X,position.Y] = view;

        return view;
    }

    public TileView Get(TilePosition position)
    {
        return _tileViews[position.X, position.Y];
    }
}
