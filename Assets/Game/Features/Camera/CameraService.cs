using Game.Application.Core;
using UnityEngine;
public interface ICameraService : IService
{
    void SetFollowTarget(Transform target);
}
public sealed class CameraService : ICameraService, ILateUpdatable
{
    private readonly CameraPresenter _presenter;
    private readonly IcameraView _view;

    public CameraService(CameraPresenter presenter, IcameraView view)
    {
        _presenter = presenter;
        _view = view;
        if (_view.Transform.parent == null)
        {
            Object.DontDestroyOnLoad(_view.Transform.gameObject);
        }
    }

    public void OnLateUpdatable(float deltaTime) => _presenter.Update(deltaTime);
    public void SetFollowTarget(Transform target) => _presenter.SetTarget(target);
}
