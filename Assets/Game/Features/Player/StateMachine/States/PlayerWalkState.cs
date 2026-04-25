using UnityEngine;

public class PlayerWalkState : PlayerLocomotionState
{
    protected override void OnEnter(PlayerContext Context)
    {
        Context.Core.Movement.SetSprint(false);
    }
}
