
public interface IReadOnlyGrid
{
    int Width { get; }
    int Height { get; }

    Tile Get(int x, int y);
}

public interface IGrid : IReadOnlyGrid
{
    void Set(int x, int y, Tile tile);
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
}