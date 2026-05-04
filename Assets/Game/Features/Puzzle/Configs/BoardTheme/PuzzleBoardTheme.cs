using UnityEngine;

[CreateAssetMenu( fileName = "PuzzleBoardTheme", menuName = "Puzzle/Board Theme/Puzzle Board Theme")]
public sealed class PuzzleBoardTheme :ScriptableObject
{
    [Header("Board")]
    public BoardCellView CellPrefab;

    public TileView TilePrefab;

    [Header("Visual Database")]
    public TileVisualDatabase TileVisualDatabase;

    [Header("Background")]
    public Sprite BoardBackground;

    [Header("Board Colors")]
    public Color BoardTint = Color.white;
}
