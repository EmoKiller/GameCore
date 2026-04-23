using UnityEngine;

public class PlayerJumpState : CharacterState<PlayerContext>
{
    protected override void OnEnter(PlayerContext Context )
    {
        Debug.Log("Jump");
        Context.Core.Movement.Jump();
        Context.Core.Animator.SetGrounded(false);
    }
    protected override void OnUpdate(PlayerContext Context ,float dt)
    {
        Context.Core.Animator.SetSpeedVertical(Context.Core.Movement.ActualVelocity.y);
        Context.Core.Animator.SetMoveSpeed(Context.Core.Movement.TargetVelocityX);
        
        if (Context.Core.Movement.ActualVelocity.y <= 0.1 && 
            Context.Core.Sensor.IsGrounded &&
             Context.CharacterInput.MoveInput.x >= 0.2
            )
        {
            Context.Core.Animator.SetGrounded(true);
            Context.StateMachine.ChangeState(ECharacterStateId.Walk);
            return;
        }



        if (Context.Core.Movement.ActualVelocity.y <= 0.1 &&
            Context.Core.Sensor.IsGrounded &&
             Context.CharacterInput.MoveInput.x <= 0.1)
        {
            Context.Core.Animator.SetGrounded(true);
            Context.StateMachine.ChangeState(ECharacterStateId.Idle);
            return;
        }
        
        //Debug.Log(Context.Core.Movement.ActualVelocity.y);
    }
    protected override void OnExit(PlayerContext Context)
    {
        Context.Core.Animator.SetSpeedVertical(0);
    }
}
