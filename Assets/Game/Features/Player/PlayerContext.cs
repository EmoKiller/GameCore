
using Game.Share.StateMachine;

public class PlayerContext 
{
    public StateMachine<ECharacterStateId, PlayerContext> StateMachine;
    public CharacterContext Core;
    public ICharacterInput CharacterInput {get ;}
    public CharacterSensor PlayerCharacterSensor{get ;}

    public FlipCharacter2D FlipCharacter{get ;}

    public PlayerContext(
        CharacterContext core,
        ICharacterInput characterInput,
        CharacterSensor playerCharacterSensor,
        FlipCharacter2D flipCharacter)
    {
        Core = core;    
        CharacterInput = characterInput;
        PlayerCharacterSensor = playerCharacterSensor;
        FlipCharacter = flipCharacter;
        
    }
}