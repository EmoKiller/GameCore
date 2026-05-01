using UnityEngine;

public sealed class PuzzleBoardView : MonoBehaviour
{
    private TileView _tilePrefab;
    private TileVisualDatabase _visualDatabase;

    private TileView[,] _tileViews;

    private IPuzzleService _service;

    private BoardLayout _layout;

    // để tạm test
    public BoardLayout Layout=> _layout;

    public void Initialize(TileView tilePrefab, TileVisualDatabase visualDatabase)
    {
        _tilePrefab = tilePrefab;
        _visualDatabase = visualDatabase;
    }

    private void OnEnable()
    {
        _service.BoardChanged += RefreshBoard;
    }

    private void OnDisable()
    {
        _service.BoardChanged -= RefreshBoard;
    }
    public void InitializeBoard(
        IPuzzleService service,
        float cellSize)
    {
        _service = service;

        _layout = new BoardLayout(cellSize);

        CreateViews();

        RefreshBoard();
    }
    private void CreateViews()
    {
        var board = _service.Board;

        _tileViews =
            new TileView[board.Width, board.Height];

        for (int y = 0; y < board.Height; y++)
        {
            for (int x = 0; x < board.Width; x++)
            {
                CreateTileView(x, y);
            }
        }
    }
    private void CreateTileView(int x, int y)
    {
        var view =
            Instantiate(_tilePrefab, transform);

        var position =
            new TilePosition(x, y);

        view.SetPosition(position);

        view.SetWorldPosition(
            _layout.GetWorldPosition(position));

        _tileViews[x, y] = view;
    }
    public void RefreshBoard()
    {
        var board = _service.Board;

        for (int y = 0; y < board.Height; y++)
        {
            for (int x = 0; x < board.Width; x++)
            {
                RefreshTile(x, y);
            }
        }
    }
    private void RefreshTile(int x, int y)
    {
        var tile = _service.Board.Get(x, y);

        var view = _tileViews[x, y];

        var sprite =
            _visualDatabase.GetSprite(tile.Type);

        view.SetSprite(sprite);
    }
    public void MoveView(
        TilePosition from,
        TilePosition to)
    {
        var view = _tileViews[from.X, from.Y];

        _tileViews[to.X, to.Y] = view;

        _tileViews[from.X, from.Y] = null;

        view.SetPosition(to);
    }
    public void SwapViews(
        TilePosition a,
        TilePosition b)
    {
        var viewA = _tileViews[a.X, a.Y];

        var viewB = _tileViews[b.X, b.Y];

        _tileViews[a.X, a.Y] = viewB;
        _tileViews[b.X, b.Y] = viewA;

        viewA.SetPosition(b);
        viewB.SetPosition(a);
    }
    public void HideView( TilePosition position)
    {
        var view = _tileViews[position.X, position.Y];

        view.gameObject.SetActive(false);

        _tileViews[position.X, position.Y] = null;
    }
    public TileView CreateOrReuseView( TilePosition position, ETileType tileType)
    {
        var view = Instantiate(_tilePrefab, transform);

        view.SetPosition(position);

        view.SetSprite( _visualDatabase.GetSprite(tileType));

        _tileViews[position.X, position.Y] = view;

        return view;
    }
    public TileView GetTileView(
        TilePosition position)
    {
        return _tileViews[position.X, position.Y];
    }
}
