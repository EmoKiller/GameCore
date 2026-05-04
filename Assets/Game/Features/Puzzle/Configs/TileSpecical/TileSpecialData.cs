using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Special Tile Data", menuName = "Puzzle/Special/Special Tile Data")]
public sealed class TileSpecialData : ScriptableObject
{
    [field: SerializeField]
    public string Id { get; private set; }

    [field: SerializeField]
    public Sprite Icon { get; private set; }

    [field: SerializeField]
    public SpecialTileBehaviour Behaviour { get; private set; }
}
