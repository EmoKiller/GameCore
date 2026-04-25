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
        var assetProvider = services.Resolve<IAssetProvider>();

        var handle = await assetProvider.LoadAsync<GameObject>("MainCamera", ct);
        
        var cameraObj = UnityEngine.Object.Instantiate(handle.Asset);
        var cameraView = cameraObj.GetComponent<CameraView>();

        var presenter = new CameraPresenter(cameraView, 0.2f);
        var cameraService = new CameraService(presenter, cameraView);

        
        GameApplication.Instance.Lifecycle.Register(cameraService);
        services.Register<ICameraService>(cameraService);

        await UniTask.CompletedTask;
    }

    public override void Shutdown()
    {

    }

    
}
