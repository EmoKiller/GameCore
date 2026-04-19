using UnityEngine;

public abstract class PlayerLocomotionState : CharacterState<PlayerContext>
{
    protected override void OnUpdate(PlayerContext Context ,float dt)
    {
        if (Context.CharacterInput.JumpHeld == true)
        {
            Context.StateMachine.ChangeState(ECharacterStateId.Jump);
            return;
        }
        Context.Core.Movement.Move(Context.CharacterInput.MoveInput);

        // Animation
        Context.Core.Animator.SetMoveSpeed(Context.Core.Movement.TargetVelocityX);

        // Facing
        Context.FlipCharacter.HandleFacing(Context.CharacterInput.MoveInput);

        // Cho state con xử lý thêm
        HandleStateTransition(Context, Context.CharacterInput.MoveInput.x);
    }

    protected abstract void HandleStateTransition(PlayerContext Context ,float input);
}
