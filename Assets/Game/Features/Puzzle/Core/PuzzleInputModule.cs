using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using UnityEngine;

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
        var puzzleService = services.Resolve<IPuzzleService>();

        var puzzleInputService = new PuzzleInputService(puzzleService);
        services.Register<IPuzzleInputService>(puzzleInputService);

        
    }
}
