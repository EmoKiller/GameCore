using UnityEngine;

public sealed class BoardLayout
{
    private readonly float _cellSize;

    public BoardLayout(float cellSize)
    {
        _cellSize = cellSize;
    }

    public Vector3 GetWorldPosition(TilePosition position)
    {
        return new Vector3(
            position.X * _cellSize,
            position.Y * _cellSize,
            0f);
    }
}
