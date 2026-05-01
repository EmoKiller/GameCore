using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using UnityEngine;

public class PuzzleGameplayModule : BaseGameModule
{
    public override string ModuleName => "PuzzleGameplayModule";

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

        var assetProvider = services.Resolve<IAssetProvider>();

        var puzzleBoardViewFactory = new PuzzleBoardViewFactory(assetProvider);

        var puzzleGameplayService = new PuzzleGameplayService(
            puzzleService,
            puzzleBoardViewFactory
        );
        services.Register<IPuzzleGameplayService>(puzzleGameplayService);

    }
}
