public interface IReadOnlyPuzzleBoard
{
    TileData Get(int x, int y);
    int Width { get; }
    int Height { get; }
}
public sealed class PuzzleBoard : IReadOnlyPuzzleBoard
{
    private readonly TileData[,] _tiles;

    public int Width { get; }

    public int Height { get; }

    public PuzzleBoard(int width, int height)
    {
        Width = width;
        Height = height;

        _tiles = new TileData[width, height];
    }

    public TileData Get(int x, int y)
    {
        return _tiles[x, y];
    }

    public TileData Get(TilePosition position)
    {
        return _tiles[position.X, position.Y];
    }

    public void Set(int x, int y, TileData tile)
    {
        _tiles[x, y] = tile;
    }

    public void Set(TilePosition position, TileData tile)
    {
        _tiles[position.X, position.Y] = tile;
    }

    public bool IsInside(int x, int y)
    {
        return x >= 0 &&
               y >= 0 &&
               x < Width &&
               y < Height;
    }

    public bool IsInside(TilePosition position)
    {
        return IsInside(position.X, position.Y);
    }
    public bool IsEmpty(TilePosition position)
    {
        return Get(position).IsEmpty;
    }
    public void Clear(TilePosition position)
    {
        Set(position,new TileData(ETileType.None));
    }
}
