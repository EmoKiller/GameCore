using Cysharp.Threading.Tasks;
using Game.Application.Core;
using UnityEngine;
public interface IPlayerService : IService
{
    Transform GetTransform();
    void SetInputEnabled(bool enabled);
    void Spawn();
    
}
public sealed class PlayerService : IPlayerService
{
    private readonly PlayerFactory _playerFactory;
    private PlayerController2D _playerController;

    private PlayerContext _playerContext;
    private PlayerStateSystem _playerStateSystem;
    

    public PlayerService( PlayerFactory playerFactory )
    {
        _playerFactory = playerFactory;
    }
    
    public void Update(float deltaTime)
    {
        _playerStateSystem.Tick(deltaTime);
    }



    public Transform GetTransform()
    {
        return _playerController.transform;
    }
    public void Spawn()
    {
        _playerController = _playerFactory.CreateAsync();
        Initialize();
    }
    private void Initialize()
    {
        var playerInputAdapter = _playerController.GetComponentInChildren<PlayerInputAdapter>();
        playerInputAdapter.Initialize();

        _playerContext = new PlayerContext(
            _playerController.CharacterContext,
            playerInputAdapter,
            _playerController.GetComponentInChildren<CharacterSensor>(),
            new FlipCharacter2D(_playerController.gameObject)
        );

        _playerStateSystem = new PlayerStateSystem(_playerContext);
        _playerContext.StateMachine = _playerStateSystem.StateMachine;

        //test sẽ chỉnh sửa logic lại APP
        GameApplication.Instance.Lifecycle.OnUpdate += Update;
    }
    public void SetInputEnabled(bool enabled)
    {
        //_context.InputEnabled = enabled;
    }

}
