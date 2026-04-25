using UnityEngine;

public abstract class PlayerLocomotionState : CharacterState<PlayerContext>
{
    protected override void OnUpdate(PlayerContext Context ,float dt)
    {
        
        Context.Core.Movement.Move(Context.Input.MoveInput);

        // Animation
        Context.Core.Animator.SetMoveSpeed(Context.Core.Movement.TargetVelocityX);

        // Facing
        Context.FlipCharacter.HandleFacing(Context.Input.MoveInput);

    }

}
