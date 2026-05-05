using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using UnityEngine;

public class PuzzleGameplayModule : BaseGameModule
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
        var puzzleService = services.Resolve<IPuzzleService>();

        var assetProvider = services.Resolve<IAssetProvider>();
        
        var boardLayout = new BoardLayout(
            puzzleService.Board.Width,
            puzzleService.Board.Height,
            GameObject.FindFirstObjectByType<Camera>(),
            boardOffset: new Vector2(0f, -4f)
        );

        var puzzleBoardViewFactory = new PuzzleBoardViewFactory(assetProvider);

        var handlePuzzleAnimationConfig = await assetProvider.LoadAsync<ScriptableObject>("PuzzleAnimationConfig", ct);
        var puzzleAnimationConfig = handlePuzzleAnimationConfig.Asset as PuzzleAnimationConfig;

        var boardAnimator = new PuzzleBoardAnimator(puzzleAnimationConfig);

        // PuzzleGameplayService
        var prefab = await assetProvider.LoadAsync<GameObject>("PuzzleGameplayService", ct);
        
        var instance = UnityEngine.Object.Instantiate(prefab.Asset, GameApplication.Instance.transform);
        var service = instance.GetComponent<PuzzleGameplayService>();

        service.Initialized(
            puzzleService,
            puzzleBoardViewFactory,
            boardAnimator,
            boardLayout
        );
        services.Register<IPuzzleGameplayService>(service);

    }
}
