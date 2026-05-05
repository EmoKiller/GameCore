using UnityEngine;

[CreateAssetMenu(
    menuName = "Puzzle/Board Preset")]
public sealed class BoardPreset : ScriptableObject
{
    [Min(1)]
    public int Width = 8;

    [Min(1)]
    public int Height = 8;

    public TilePresetData[] Tiles;
}
