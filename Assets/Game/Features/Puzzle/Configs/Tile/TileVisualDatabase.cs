using UnityEngine;

[CreateAssetMenu(menuName = "Puzzle/Tile/Tile Sprite Visual Database")]
public sealed class TileVisualDatabase : ScriptableObject
{
    [SerializeField]
    private TileVisualEntry[] _entries;

    public Sprite GetSprite(ETileType type)
    {
        foreach (var entry in _entries)
        {
            if (entry.Type == type)
            {
                return entry.Sprite;
            }
        }

        return null;
    }
}
