using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core.SceneLoader;
using Game.Application.Events;
using Game.Presentation.UI.View;
using UnityEngine;

public class PuzzleGameStateContext : IGameFlowContext , ILoadingStateContext
{
    public EGameState NextGameState{ get; private set; }
    public bool IsLoadingCompleted { get; private set; }
    
    public IEventBus EventBus { get; }
    public ISceneLoader SceneLoader { get; }
    public IUIService UIService { get; }
    public IAssetProvider AssetProvider { get; }
    public IPuzzleSystem PuzzleSystem { get; }
    
    public PuzzleGameStateContext(
        ISceneLoader sceneLoader,
        IUIService uiService,
        IEventBus eventBus,
        IAssetProvider assetProvider,
        IPuzzleSystem puzzleSystem
    )
    {
        SceneLoader = sceneLoader;
        UIService = uiService;
        EventBus = eventBus;
        AssetProvider = assetProvider;
        PuzzleSystem = puzzleSystem;
    }

    public void SetNextState(EGameState state)
    {
        NextGameState = state;
        IsLoadingCompleted = false;
    }
    public void MarkLoadingAsCompleted()
    {
        IsLoadingCompleted = true;
    }

    public async UniTask ShowLoading(CancellationToken ct)
    {
        await UIService.ShowAsync<LoadingView>(ct);
    }

    public async UniTask HideLoading(CancellationToken ct)
    {
        await UIService.HideAsync<LoadingView>(ct);
    }
}
