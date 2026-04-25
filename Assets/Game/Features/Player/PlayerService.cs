using Cysharp.Threading.Tasks;
using Game.Application.Core;
using Game.Application.Core.Input;
using Game.Character.Core;
using UnityEngine;
public interface IPlayerService : IService
{
    Transform GetTransform();
    void SetInputEnabled(bool enabled);
    void Spawn();
    
}
public sealed class PlayerService : IPlayerService, IUpdatable
{
    private readonly PlayerFactory _playerFactory;
    private readonly IInputService _inputService;
    private readonly CharacterStatsConfig _characterStatsConfig;

    private PlayerPresenter _playerPresenter;
    private ICharacterView _characterView;
    
    

    public PlayerService( 
        PlayerFactory playerFactory,
        IInputService inputService,
        CharacterStatsConfig characterStatsConfig
    )
    {
        _playerFactory = playerFactory;
        _inputService = inputService;
        _characterStatsConfig = characterStatsConfig;
    }
    public Transform GetTransform()
    {
        return _characterView.GetTransform();
    }
    public void Spawn()
    {
        _characterView = _playerFactory.Create();
        Initialize();
        _playerPresenter.SetView(_characterView);
        
    }
    private void Initialize()
    {
        // input
        var playerInputAdapter = _inputService.GetPlayerInput();
        playerInputAdapter.Initialize();

        // CharacterModel
        var characterModel = new CharacterModel(_characterStatsConfig);

        // PlayerContext
        var _playerContext = new PlayerContext(
            _characterView.CharacterContext,
            characterModel,
            null,
            playerInputAdapter,
            new FlipCharacter2D(_characterView.GetGameObject())
        );

        // PlayerStateSystem
        var playerStateSystem = new PlayerStateSystem(_playerContext);

        _playerPresenter = new PlayerPresenter(
            _playerContext,
            playerStateSystem
        );
        //test sẽ chỉnh sửa logic lại APP
        GameApplication.Instance.Lifecycle.Register(this);
    }
    public void SetInputEnabled(bool enabled)
    {
        //_context.InputEnabled = enabled;
    }

    public void OnUpdate(float deltaTime)
    {
        _playerPresenter.Update(deltaTime);
        
    }
}
