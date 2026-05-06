using UnityEngine;

public struct TileData
{
    public ETileType Type;

    public TileSpecialData Special;
    public TileRuntimeSpecialState RuntimeSpecialState;

    public TileData(
        ETileType type,
        TileSpecialData special = null
    )
    {
        Type = type;

        Special = special;
        RuntimeSpecialState = special?.Behaviour?.CreateRuntimeState();
    }

    public readonly bool IsEmpty => Type == ETileType.None;

    public readonly bool HasSpecial => Special != null;
    
}