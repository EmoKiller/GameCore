using UnityEngine;

public sealed class PuzzleBoardView : MonoBehaviour
{
    // [SerializeField]
    // private PuzzleBoardTheme _theme;
    
    
    private IPuzzleService _service;

    private BoardCellLayer _cellLayer;

    private TileLayer _tileLayer;

    private IReadOnlyBoardLayout _layout;
    public IReadOnlyBoardLayout Layout => _layout;
    [SerializeField]
    private BoardBackgroundView _boardBackgroundView;

    private void OnDisable()
    {
        _service.BoardChanged -= RefreshBoard;
    }
    public void InitializeBoard(
        IPuzzleService service,
        IReadOnlyBoardLayout boardLayout,
        BoardCellView cellPrefab,
        TileView tilePrefab,
        TileVisualDatabase visualDatabase)
    {
        _service = service;
        _service.BoardChanged += RefreshBoard;

        _layout = boardLayout;

        _cellLayer = new BoardCellLayer(cellPrefab , _layout , transform);
        _tileLayer = new TileLayer(tilePrefab, transform, _layout, visualDatabase);

        // backGround
        float width = service.Board.Width * _layout.TileSize;
        float height = service.Board.Height * _layout.TileSize;
        _boardBackgroundView.SetSize(width, height);

        CreateViews();
        RefreshBoard();
    }
    private void CreateViews()
    {
        IReadOnlyPuzzleBoard board =
            _service.Board;

        _cellLayer.Initialize(board);

        _tileLayer.Initialize(board);

        for (int y = 0; y < board.Height; y++)
        {
            for (int x = 0; x < board.Width; x++)
            {
                _cellLayer.Create(x, y);

                _tileLayer.Create(x, y);
            }
        }
    }
    public void RefreshBoard()
    {
        IReadOnlyPuzzleBoard board =
            _service.Board;

        for (int y = 0; y < board.Height; y++)
        {
            for (int x = 0; x < board.Width; x++)
            {
                _tileLayer.Refresh(board, x, y);
            }
        }
    }
    public void MoveView( TilePosition from,  TilePosition to)
    {
        _tileLayer.Move(from, to);
    }

    public void SwapViews( TilePosition a, TilePosition b)
    {
        _tileLayer.Swap(a, b);
    }

    public void HideView( TilePosition position)
    {
        _tileLayer.Hide(position);
    }

    public TileView CreateOrReuseView( TilePosition position, ETileType tileType)
    {
        return _tileLayer.CreateOrReuse(position, tileType);
    }
    public TileView GetTileView(TilePosition position)
    {
        return _tileLayer.Get(position);
    }
    public bool IsInside(TilePosition position)
    {
        return
            position.X >= 0 &&
            position.Y >= 0 &&
            position.X < _service.Board.Width &&
            position.Y < _service.Board.Height;
    }
}
