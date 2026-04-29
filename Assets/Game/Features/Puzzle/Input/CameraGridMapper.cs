using UnityEngine;
public interface IGridMapper
{
    bool TryMap(Vector2 screenPosition, out GridPosition gridPos);
}
public sealed class CameraGridMapper : IGridMapper
{
    private readonly Camera _camera;

    public CameraGridMapper(Camera camera)
    {
        _camera = camera;
    }

    public bool TryMap(Vector2 screenPosition, out GridPosition gridPos)
    {
        var world = _camera.ScreenToWorldPoint(screenPosition);

        gridPos = new GridPosition(
            Mathf.FloorToInt(world.x),
            Mathf.FloorToInt(world.y)
        );

        return true;
    }
}
