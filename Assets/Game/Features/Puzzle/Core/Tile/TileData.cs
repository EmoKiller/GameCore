using UnityEngine;

public struct TileData
{
    public ETileType Type;

    public TileSpecialData Special;

    public TileData(
        ETileType type,
        TileSpecialData special = null
    )
    {
        Type = type;

        Special = special;
    }

    public readonly bool IsEmpty => Type == ETileType.None;

    public readonly bool HasSpecial => Special != null;
}