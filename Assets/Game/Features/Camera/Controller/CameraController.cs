using System;
using UnityEngine;
[Serializable]
public sealed class CameraController
{
    private Transform _cameraTransform;
    private Transform _target;

    private readonly float _smoothSpeed;
    private Vector3 _velocity;

    public CameraController(
        Transform cameraTransform,
        float smoothSpeed)
    {
        _cameraTransform = cameraTransform;
        _smoothSpeed = smoothSpeed;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void Update(float deltaTime)
    {
        if (_target == null) return;
        
        var targetPosition = _target.position + new Vector3(0,3,0);

        var newPosition = Vector3.SmoothDamp(
            _cameraTransform.position,
            targetPosition,
            ref _velocity,
            _smoothSpeed,
            Mathf.Infinity,
            deltaTime
        );

        _cameraTransform.position = new Vector3(
            newPosition.x,
            newPosition.y,
            _cameraTransform.position.z // giữ z
        );
    }
}