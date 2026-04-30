using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Puzzle/Tile Sprite Database")]
public sealed class TileSpriteDatabase : ScriptableObject
{
    [System.Serializable]
    private struct Entry
    {
        public ETileType Type;
        public Sprite Sprite;
    }

    [SerializeField] private Entry[] _entries;

    private Dictionary<ETileType, Sprite> _lookup;

    public Sprite Get(ETileType type)
    {
        if (_lookup == null)
        {
            _lookup = new Dictionary<ETileType, Sprite>();

            foreach (var e in _entries)
                _lookup[e.Type] = e.Sprite;
        }

        if (!_lookup.TryGetValue(type, out var sprite))
            throw new System.Exception($"Missing sprite for {type}");

        return sprite;
    }
}