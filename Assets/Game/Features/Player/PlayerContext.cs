
using Game.Share.StateMachine;

public class PlayerContext 
{
    public StateMachine<ECharacterStateId, PlayerContext> StateMachine;
    public CharacterContext Core;
    public ICharacterInput CharacterInput {get ;}
    public FlipCharacter2D FlipCharacter{get ;}

    public PlayerContext(
        CharacterContext core,
        ICharacterInput characterInput,
        FlipCharacter2D flipCharacter)
    {
        Core = core;    
        CharacterInput = characterInput;
        FlipCharacter = flipCharacter;
        
    }
}