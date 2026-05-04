

using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core.SceneLoader;
using Game.Application.Events;
using Game.Application.UI;
using Game.Presentation.UI.View;

public class RPGGameStateContext : IGameFlowContext , ILoadingStateContext
{
    public EGameState NextGameState{ get; private set; }
    public bool IsLoadingCompleted { get; private set; }
    
    public IEventBus EventBus { get; }
    public ISceneLoader SceneLoader { get; }
    public IUIService UIService { get; }
    public IAssetProvider AssetProvider { get; }
    public IPlayerService PlayerService {get;}
    public ICameraService CameraService {get;}
    
    public RPGGameStateContext(
        ISceneLoader sceneLoader,
        IUIService uiService,
        IEventBus eventBus,
        IAssetProvider assetProvider,
        IPlayerService playerService,
        ICameraService cameraService)
    {
        SceneLoader = sceneLoader;
        UIService = uiService;
        EventBus = eventBus;
        AssetProvider = assetProvider;
        PlayerService = playerService;
        CameraService = cameraService;
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
