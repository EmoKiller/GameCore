public interface IGrid
{
    int Width { get; }
    int Height { get; }

    Tile Get(int x, int y);
    void Set(int x, int y, Tile tile);

    void Swap(int x1, int y1, int x2, int y2);
}
public sealed class Grid : IGrid
{
    private readonly Tile[,] _tiles;

    public int Width { get; }
    public int Height { get; }

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        _tiles = new Tile[width, height];
    }

    public Tile Get(int x, int y) => _tiles[x, y];

    public void Set(int x, int y, Tile tile)
    {
        _tiles[x, y] = tile;
    }

    public void Swap(int x1, int y1, int x2, int y2)
    {
        var temp = _tiles[x1, y1];
        _tiles[x1, y1] = _tiles[x2, y2];
        _tiles[x2, y2] = temp;
    }
}