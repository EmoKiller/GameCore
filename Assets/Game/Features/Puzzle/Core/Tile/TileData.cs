using UnityEngine;

public struct TileData
{
    public ETileType Type;

    public TileData(ETileType type)
    {
        Type = type;
    }
    public readonly bool IsEmpty => Type == ETileType.None;
}
