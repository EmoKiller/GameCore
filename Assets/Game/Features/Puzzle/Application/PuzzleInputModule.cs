using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleInputModule : BaseGameModule
{
    public override Type[] GetDependencies()
    {
        return new Type[]
        {
            typeof(PuzzleModule),
        };
    }

    public override void Shutdown()
    {
        
    }

    protected override async UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct)
    {
        var assetProvider = services.Resolve<IAssetProvider>();


        var puzzleGameplayService = services.Resolve<IPuzzleGameplayService>();
        var applicationLifecycle = services.Resolve<IApplicationLifecycle>();

        var playerInput = GameObject.FindFirstObjectByType<PlayerInput>();
        var camera = GameObject.FindFirstObjectByType<Camera>();

        var pointerInputReader = new InputSystemPointerReader(playerInput);

        // PuzzleInputService
        var prefab = await assetProvider.LoadAsync<GameObject>("PuzzleInputService", ct);
        
        var instance = UnityEngine.Object.Instantiate(prefab.Asset, GameApplication.Instance.transform);
        var service = instance.GetComponent<PuzzleInputService>();

        service.Initialized(
            puzzleGameplayService,
            pointerInputReader,
            camera
        );
        
        services.Register<IPuzzleInputService>(service);
        
        applicationLifecycle.Register(service);
        
    }
}
