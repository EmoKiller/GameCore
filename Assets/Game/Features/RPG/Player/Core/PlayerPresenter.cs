using Game.Character.Core;
using Game.Character.Core.Stats;
using UnityEngine;

public class PlayerPresenter 
{
    private ICharacterView _characterView;
    private PlayerContext _playerContext;
    
    public PlayerPresenter(
        PlayerContext playerContext
    )
    {
        _playerContext = playerContext;

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
        
    }

}
