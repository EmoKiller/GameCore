using UnityEngine;

public class PlayerFallState : CharacterState<PlayerContext>
{
    protected override void OnEnter(PlayerContext context)
    {
    }
    protected override void OnUpdate(PlayerContext context, float dt)
    {
        context.Actions.SetAnimatorSpeed();
        context.Actions.SetSpeedVertical();
        
    }
    protected override void OnExit(PlayerContext context)
    {
        //context.Core.Animator.SetSpeedVertical(0);
        //context.Core.Animator.SetGrounded(true);
    }
}
