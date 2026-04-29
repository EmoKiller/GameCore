using UnityEngine;

public interface ICharacterInput 
{
    Vector2 MoveInput { get; }
    bool SprintHeld { get; }
    bool JumpHeld { get; }
    
    event System.Action OnJump;
    event System.Action OnAttack;
}
