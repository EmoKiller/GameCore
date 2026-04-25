using UnityEngine;

public class PlayerRunState : PlayerLocomotionState
{
    protected override void OnEnter(PlayerContext Context)
    {
        Context.Core.Movement.SetSprint(true);
    }
}
