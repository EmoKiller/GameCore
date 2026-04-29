using UnityEngine;

public sealed class BoardView : MonoBehaviour
{
    [SerializeField] private TileView _tilePrefab;

    private TileView[,] _tiles;
    private IGrid _grid;

    public void Initialize(IGrid grid)
    {
        _grid = grid;

        _tiles = new TileView[grid.Width, grid.Height];

        for (int x = 0; x < grid.Width; x++)
        for (int y = 0; y < grid.Height; y++)
        {
            var tile = Instantiate(_tilePrefab, transform);
            tile.transform.localPosition = new Vector3(x, y, 0);

            _tiles[x, y] = tile;
        }

        Render();
    }

    public void Render()
    {
        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                var tile = _grid.Get(x, y);
                _tiles[x, y].SetColor(tile.Type);
            }
        }
        
    }
}