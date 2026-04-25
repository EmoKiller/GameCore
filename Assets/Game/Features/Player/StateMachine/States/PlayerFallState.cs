using UnityEngine;

public class PlayerFallState : CharacterState<PlayerContext>
{
    protected override void OnEnter(PlayerContext context)
    {
        context.Core.Animator.SetGrounded(false);
    }
    protected override void OnUpdate(PlayerContext context, float dt)
    {
        context.Core.Animator.SetSpeedVertical(context.Core.Movement.ActualVelocity.y);
        context.Core.Animator.SetMoveSpeed(context.Core.Movement.TargetVelocityX);
    }
    protected override void OnExit(PlayerContext context)
    {
        context.Core.Animator.SetSpeedVertical(0);
        context.Core.Animator.SetGrounded(true);
    }
}
