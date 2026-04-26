using UnityEngine;

public class PlayerJumpState : CharacterState<PlayerContext>
{
    protected override void OnEnter(PlayerContext context )
    {
        context.Actions.Jump();
    }
    protected override void OnUpdate(PlayerContext context ,float dt)
    {
        context.Actions.SetAnimatorSpeed();
        context.Actions.SetSpeedVertical();
        
    }
    protected override void OnExit(PlayerContext Context)
    {
        //Context.Core.Animator.SetSpeedVertical(0);
    }
}
