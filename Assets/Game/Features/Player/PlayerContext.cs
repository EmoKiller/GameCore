

using Game.Character.Core;
using Game.Character.Core.Stats;

public class PlayerContext 
{
    public CharacterContext Core{get ;}
    public CharacterModel Model{get ;}
    public CharacterStats Stats{get ;}
    public ICharacterInput Input {get ;}
    public FlipCharacter2D FlipCharacter{get ;}

    public PlayerContext(
        CharacterContext core,
        CharacterModel model,
        CharacterStats stats,
        ICharacterInput input,
        FlipCharacter2D flipCharacter)
    {
        Core = core;    
        Model = model;
        Stats = stats;

        Input = input;
        FlipCharacter = flipCharacter;
        
    }
}