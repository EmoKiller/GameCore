

using Game.Character.Core;
using Game.Character.Core.Stats;

public class PlayerContext 
{
    public ICharacterActions Actions {get ;}
    public ICharacterStatsFacade Stats {get ;}
    public ICharacterInput Input {get ;}
    public FlipCharacter2D FlipCharacter {get ;}

    public PlayerContext(
        ICharacterActions actions,
        ICharacterStatsFacade stats,
        ICharacterInput input,
        FlipCharacter2D flipCharacter)
    {
        Actions  = actions;    
        Stats = stats;
        Input = input;
        FlipCharacter = flipCharacter;
    }
}