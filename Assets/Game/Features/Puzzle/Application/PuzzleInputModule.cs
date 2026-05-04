using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleInputModule : BaseGameModule
{
    public override string ModuleName => "PuzzleInputModule";

    public override int InitializationOrder => 10;

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
        var puzzleGameplayService = services.Resolve<IPuzzleGameplayService>();
        var applicationLifecycle = services.Resolve<IApplicationLifecycle>();

        var playerInput = GameObject.FindFirstObjectByType<PlayerInput>();
        var camera = GameObject.FindFirstObjectByType<Camera>();

        var pointerInputReader = new InputSystemPointerReader(playerInput);

        var puzzleInputService = new PuzzleInputService(puzzleGameplayService, pointerInputReader, camera);
        services.Register<IPuzzleInputService>(puzzleInputService);
        
        applicationLifecycle.Register(puzzleInputService);
        
    }
}
