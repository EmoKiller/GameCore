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
        var handlePuzzleBoardView = await assetProvider.LoadAsync<GameObject>("PuzzleBoardView", ct);
        var handleTileView = await assetProvider.LoadAsync<GameObject>("TileView", ct);
        var handleTileVisualDatabase = await assetProvider.LoadAsync<GameObject>("TileVisualDatabase", ct);

        var tileView = handleTileView.Asset.GetComponent<TileView>();
        var tileVisualDatabase = handleTileVisualDatabase.Asset.GetComponent<TileVisualDatabase>();
        var puzzleBoardView = handlePuzzleBoardView.Asset.GetComponent<PuzzleBoardView>();

        puzzleBoardView.Initialize(tileView, tileVisualDatabase);


        var puzzleBoardViewFactory = new PuzzleBoardViewFactory(puzzleBoardView);


        var puzzleGameplayService = new PuzzleGameplayService(
            puzzleService,
            puzzleBoardViewFactory
        );
        services.Register<IPuzzleGameplayService>(puzzleGameplayService);

    }
}
