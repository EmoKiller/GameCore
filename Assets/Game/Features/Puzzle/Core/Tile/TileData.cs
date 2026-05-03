using UnityEngine;

public struct TileData
{
    public ETileType Type;

    public ETileSpecialType SpecialType;

    public TileData(
        ETileType type,
        ETileSpecialType specialType = ETileSpecialType.None
    )
    {
        Type = type;

        SpecialType = specialType;
    }

    public readonly bool IsEmpty => Type == ETileType.None;

    public readonly bool HasSpecial => SpecialType != ETileSpecialType.None;
}