using Game.Application.Core;
using UnityEngine;
public interface ICameraService : IService
{
    void SetFollowTarget(Transform target);
}
public sealed class CameraService : ICameraService
{
    private readonly CameraController _controller;

    public CameraService(CameraController controller)
    {
        _controller = controller;
    }

    public void SetFollowTarget(Transform target)
    {
        _controller.SetTarget(target);
    }

}
