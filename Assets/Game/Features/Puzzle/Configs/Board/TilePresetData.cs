using UnityEngine;

[System.Serializable]
public struct TilePresetData
{
    public Vector2Int Position;

    public ETileType TileType;

    public TileSpecialData Special;
}