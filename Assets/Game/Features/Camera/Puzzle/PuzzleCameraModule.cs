using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using UnityEngine;

public class PuzzleCameraModule : BaseGameModule
{
    public override string ModuleName => "PuzzleCameraModule";

    public override int InitializationOrder => 0;

    public override Type[] GetDependencies() => Type.EmptyTypes;

    protected override async UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct)
    {
        var assetProvider = services.Resolve<IAssetProvider>();

        var handle = await assetProvider.LoadAsync<GameObject>("MainCamera", ct);

        var cameraObj = UnityEngine.Object.Instantiate(handle.Asset);
        var cameraView = cameraObj.GetComponent<CameraView>();


        var cameraService = new PuzzleCameraService();
        
        services.Register<IPuzzleCameraService>(cameraService);
    }
    public override void Shutdown()
    {
        
    }

}
