using UnityEngine;

public readonly struct Tile
{
    public ETileType Type { get; }
    public bool IsEmpty { get; }

    public Tile(ETileType type)
    {
        Type = type;
        IsEmpty = false;
    }

    private Tile(bool empty)
    {
        Type = default;
        IsEmpty = true;
    }

    public static Tile Empty => new Tile(true);
}
