using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using UnityEngine;

public class PuzzleInputModule : BaseGameModule
{
    public override string ModuleName => "PuzzleInputModule";

    public override int InitializationOrder => 20;

    public override Type[] GetDependencies()
    {
        return new Type[]
        {
            typeof(PuzzleModule),
        };
    }
    private IPointerInputSystem _pointer;
    protected override async UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct)
    {
        var puzzle = services.Resolve<IPuzzleSystem>();
        
        // input
        var inputService = new UnityInputService();
        services.Register<IInputPuzzleService>(inputService);

        //point
        _pointer = new PointerInputSystem(inputService);

        var camera = UnityEngine.Camera.main;

    }
    public override void Shutdown()
    {
        
    }

    
}
