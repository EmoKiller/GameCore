using Game.Character.Core;
using Game.Character.Core.Stats;
using UnityEngine;

public class PlayerPresenter 
{
    private ICharacterView _characterView;
    private PlayerContext _playerContext;
    private PlayerStateSystem _playerStateSystem;
    public PlayerPresenter(
        PlayerContext playerContext,
        PlayerStateSystem playerStateSystem
    )
    {
        _playerContext = playerContext;
        _playerStateSystem = playerStateSystem;

    }
    public void Init()
    {
        
    }
    public void SetView(ICharacterView caracterView)
    {
        _characterView = caracterView;
    }
    public void Update(float deltaTime)
    {
        _playerStateSystem.Tick(deltaTime);
    }

}
