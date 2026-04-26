using UnityEngine;

public class PlayerRunState : PlayerLocomotionState
{
    protected override void OnEnter(PlayerContext context)
    {
        //Context.Core.Movement.SetSprint(true);
        context.Actions.SetSprint(true);
    }
}
