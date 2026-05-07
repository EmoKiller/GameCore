using UnityEngine;

public sealed class TileData
{
    public ETileType Type;

    public TilePosition Position;

    public TileSpecialData Special;

    public TileRuntimeSpecialState RuntimeSpecialState;
    public bool IsEmpty => Type == ETileType.None;
    public bool HasSpecial => Special != null;
    
    public TileData(
        ETileType type,
        TileSpecialData special = null
    )
    {
        Type = type;
        Position = TilePosition.Invalid;
        Special = special;
        RuntimeSpecialState = special?.Behaviour?.CreateRuntimeState();
    }
}