using UnityEngine;
public interface IWorldPointerMapper
{
    Vector3 ScreenToWorld(Vector2 screenPosition);
}
public sealed class WorldPointerMapper : IWorldPointerMapper
{
    private readonly Camera _camera;

    public WorldPointerMapper(Camera camera)
    {
        _camera = camera;
    }

    public Vector3 ScreenToWorld(Vector2 screenPosition)
    {
        return _camera.ScreenToWorldPoint(
            new Vector3(
                screenPosition.x,
                screenPosition.y,
                Mathf.Abs(_camera.transform.position.z)
            )
        );
    }
}
