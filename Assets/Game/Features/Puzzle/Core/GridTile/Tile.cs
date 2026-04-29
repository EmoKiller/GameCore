using UnityEngine;

public readonly struct Tile
{
    public ETileType Type { get; }

    public Tile(ETileType type)
    {
        Type = type;
    }
}
