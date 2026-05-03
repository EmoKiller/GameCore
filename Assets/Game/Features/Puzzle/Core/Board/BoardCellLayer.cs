using UnityEngine;

public class BoardCellLayer 
{
    private readonly BoardCellView _cellPrefab;
    private BoardCellView[,] _cellViews;
    private readonly IReadOnlyBoardLayout _layout;
    private readonly Transform _parent;

    public BoardCellLayer(BoardCellView cellPrefab , IReadOnlyBoardLayout layout, Transform parent)
    {
        _cellPrefab = cellPrefab;
        _layout = layout;
        _parent = parent;
    }
    public void Initialize(IReadOnlyPuzzleBoard board)
    {
        _cellViews = new BoardCellView[board.Width, board.Height];
    }
    public void Create(int x, int y)
    {
        var position = new TilePosition(x, y);

        var view = GameObject.Instantiate(_cellPrefab, _parent);

        view.SetWorldPosition(_layout.GetWorldPosition(position));

        view.SetSize(_layout.TileSize);

        view.gameObject.name = $"Cell ({x}, {y})";

        _cellViews[x, y] = view;
    }
}
