

using Game.Application.Core.SceneLoader;
using Game.Application.Events;
using Game.Application.UI;

public class GameStateContext 
{
    public EGameState NextGameState{ get; private set; }
    public bool IsLoadingCompleted { get; private set; }
    
    public IEventBus EventBus { get; }
    public ISceneLoader SceneLoader { get; }
    public IUIService UIService { get; }
    public IAssetProvider AssetProvider { get; }
    public IPlayerService PlayerService {get;}
    public ICameraService CameraService {get;}
    
    public GameStateContext(
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
}
