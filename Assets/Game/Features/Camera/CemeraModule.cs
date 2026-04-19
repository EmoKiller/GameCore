using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using UnityEngine;

public class CemeraModule : BaseGameModule
{
    public override string ModuleName => "CemeraModule";

    public override int InitializationOrder => 0;

    public override Type[] GetDependencies() => Type.EmptyTypes;

    protected override async UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct)
    {
        var camera = Camera.main;
        var adapter = camera.GetComponent<CameraAdapter>();

        var _controller = new CameraController(
            camera.transform,
            0.2f
        );

        adapter.Initialize(_controller);

        var cameraService = new CameraService(_controller);
        services.Register<ICameraService>(cameraService);

        await UniTask.CompletedTask;
    }

    public override void Shutdown()
    {

    }

    
}
