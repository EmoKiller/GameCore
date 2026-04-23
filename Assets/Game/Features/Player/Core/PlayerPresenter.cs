using Game.Character.Core;
using UnityEngine;

public class PlayerPresenter 
{
    private ICharacterView _characterView;
    private CharacterModel _characterModel;
    
    private PlayerContext _playerContext;
    private PlayerStateSystem _playerStateSystem;
    public PlayerPresenter(
        CharacterModel characterModel,
        PlayerContext playerContext,
        PlayerStateSystem playerStateSystem
    )
    {
        _characterModel = characterModel;
        _playerContext = playerContext;
        _playerStateSystem = playerStateSystem;

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
