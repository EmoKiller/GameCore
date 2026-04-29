using UnityEngine;

public class PlayerWalkState : PlayerLocomotionState
{
    protected override void OnEnter(PlayerContext context)
    {
        //Context.Core.Movement.SetSprint(false);
        context.Actions.SetSprint(false);
    }
}
