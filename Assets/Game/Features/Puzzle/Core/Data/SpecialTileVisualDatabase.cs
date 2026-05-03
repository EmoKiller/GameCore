using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "SpecialTileVisualDatabase", menuName = "Puzzle/Special Tile Visual Database")]
public sealed class SpecialTileVisualDatabase : ScriptableObject
{
    [SerializeField]
    private List<SpecialTileVisualEntry> _entries;

    public Sprite GetSprite(
        ETileType tileType,
        ETileSpecialType specialType)
    {
        foreach (var entry in _entries)
        {
            if (entry.TileType != tileType)
            {
                continue;
            }

            if (entry.SpecialType != specialType)
            {
                continue;
            }

            return entry.Sprite;
        }

        return null;
    }
}