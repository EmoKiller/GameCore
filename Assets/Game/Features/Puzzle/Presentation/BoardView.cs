using UnityEngine;

public sealed class BoardView : MonoBehaviour
{
    [SerializeField] private CellView _cellPrefab;
    [SerializeField] private TileView _tilePrefab;

    private CellView[,] _cells;
    private TileView[,] _tiles;
    private IReadOnlyGrid _grid;
    public void Initialize(IReadOnlyGrid grid)
    {
        _grid = grid;

        int w = grid.Width;
        int h = grid.Height;

        _cells = new CellView[w, h];
        _tiles = new TileView[w, h];

        // 🔹 create cells (background)
        for (int x = 0; x < w; x++)
        for (int y = 0; y < h; y++)
        {
            var cell = Instantiate(_cellPrefab, transform);
            cell.transform.localPosition = new Vector3(x, y, 0);

            _cells[x, y] = cell;
        }

        // 🔹 create tiles (content)
        for (int x = 0; x < w; x++)
        for (int y = 0; y < h; y++)
        {
            var tileData = _grid.Get(x, y);

            if (tileData.IsEmpty)
                continue;

            var tile = Instantiate(_tilePrefab, transform);
            tile.SetType(tileData.Type);
            tile.SetPosition(new Vector3(x, y, 0));

            _tiles[x, y] = tile;
            _cells[x, y].SetTile(tile);
        }
    }
    public void RenderFull()
    {
        for (int x = 0; x < _grid.Width; x++)
        for (int y = 0; y < _grid.Height; y++)
        {
            UpdateTile(x, y);
        }
    }
    public void UpdateTile(int x, int y)
    {
        var data = _grid.Get(x, y);
        var current = _tiles[x, y];

        if (data.IsEmpty)
        {
            if (current != null)
            {
                Destroy(current.gameObject);
                _tiles[x, y] = null;
            }
            return;
        }

        if (current == null)
        {
            // spawn new tile
            var tile = Instantiate(_tilePrefab, transform);
            tile.SetType(data.Type);
            tile.SetPosition(new Vector3(x, y, 0));

            _tiles[x, y] = tile;
            _cells[x, y].SetTile(tile);
        }
        else
        {
            // update type
            current.SetType(data.Type);
        }
    }
    public TileView GetTile(int x, int y)
    {
        return _tiles[x, y];
    }
    public void SetTile(int x, int y, TileView tile)
    {
        _tiles[x, y] = tile;
    }
}