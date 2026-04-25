using UnityEngine;

public class PlayerJumpState : CharacterState<PlayerContext>
{
    protected override void OnEnter(PlayerContext Context )
    {
        //Debug.Log("Jump");
        Context.Core.Movement.Jump();
        Context.Core.Animator.SetGrounded(false);
    }
    protected override void OnUpdate(PlayerContext Context ,float dt)
    {
        Context.Core.Animator.SetSpeedVertical(Context.Core.Movement.ActualVelocity.y);
        Context.Core.Animator.SetMoveSpeed(Context.Core.Movement.TargetVelocityX);
    }
    protected override void OnExit(PlayerContext Context)
    {
        Context.Core.Animator.SetSpeedVertical(0);
    }
}
