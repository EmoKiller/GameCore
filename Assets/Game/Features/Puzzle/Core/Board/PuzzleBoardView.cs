using UnityEngine;

public sealed class PuzzleBoardView : MonoBehaviour
{
    // [SerializeField]
    // private PuzzleBoardTheme _theme;
    
    
    private IPuzzleService _puzzleService;
    public IReadOnlyPuzzleBoard Board => _puzzleService.Board;

    private BoardCellLayer _cellLayer;

    private TileLayer _tileLayer;
    private IReadOnlyBoardLayout _layout;
    public IReadOnlyBoardLayout Layout => _layout;

    [SerializeField]
    private BoardBackgroundView _boardBackgroundView;

    private void OnDisable()
    {
        _puzzleService.BoardChanged -= RefreshBoard;
    }
    public void InitializeBoard(
        IPuzzleService service,
        IReadOnlyBoardLayout boardLayout,
        BoardCellView cellPrefab,
        TileView tilePrefab,
        TileVisualDatabase tilevisualDatabase,
        SpecialTileVisualDatabase specialTileVisualDatabase
        )
    {
        _puzzleService = service;
        _puzzleService.BoardChanged += RefreshBoard;

        _layout = boardLayout;

        _cellLayer = new BoardCellLayer(cellPrefab , _layout , transform);
        _tileLayer = new TileLayer(tilePrefab, transform, _layout, tilevisualDatabase, specialTileVisualDatabase);

        // backGround
        float width = service.Board.Width * _layout.TileSize;
        float height = service.Board.Height * _layout.TileSize;
        _boardBackgroundView.SetSize(width, height);

        CreateViews();
        RefreshBoard();
    }
    private void CreateViews()
    {

        _cellLayer.Initialize(Board);

        _tileLayer.Initialize(Board);

        for (int y = 0; y < Board.Height; y++)
        {
            for (int x = 0; x < Board.Width; x++)
            {
                _cellLayer.Create(x, y);

                _tileLayer.Create(x, y);
            }
        }
    }
    public void RefreshBoard()
    {
        for (int y = 0; y < Board.Height; y++)
        {
            for (int x = 0; x < Board.Width; x++)
            {
                _tileLayer.Refresh(Board, x, y);
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
}
