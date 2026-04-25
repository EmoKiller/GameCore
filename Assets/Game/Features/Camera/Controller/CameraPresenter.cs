using System;
using UnityEngine;
[Serializable]
public sealed class CameraPresenter
{
    private readonly IcameraView _view; 
    private Transform _target;
    private readonly float _smoothSpeed;
    private Vector3 _velocity;

    public CameraPresenter(IcameraView view, float smoothSpeed)
    {
        _view = view;
        _smoothSpeed = smoothSpeed;
    }

    public void SetTarget(Transform target) => _target = target;

    public void Update(float deltaTime)
    {
        if (_target == null || _view.Transform == null) return;
        
        var targetPosition = _target.position + new Vector3(0,3,0);

        var newPosition = Vector3.SmoothDamp(
            _view.Transform.position,
            targetPosition,
            ref _velocity,
            _smoothSpeed,
            Mathf.Infinity,
            deltaTime
        );

        _view.Transform.position = new Vector3(
            newPosition.x,
            newPosition.y,
            _view.Transform.position.z 
        );
    }
}