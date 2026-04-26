

using Game.Character.Core;
using Game.Character.Core.Stats;

public class PlayerContext 
{
    public ICharacterActions Actions {get ;}
    public IReadOnlyCharacterStats Stats {get ;}
    public ICharacterInput Input {get ;}
    public FlipCharacter2D FlipCharacter {get ;}

    public PlayerContext(
        ICharacterActions actions,
        IReadOnlyCharacterStats stats,
        ICharacterInput input,
        FlipCharacter2D flipCharacter)
    {
        Actions  = actions;    
        Stats = stats;
        Input = input;
        FlipCharacter = flipCharacter;
    }
}