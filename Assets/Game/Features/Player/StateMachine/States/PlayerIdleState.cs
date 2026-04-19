using UnityEngine;

public class PlayerIdleState : PlayerLocomotionState
{
    protected override void OnEnter(PlayerContext Context )
    {
        Debug.Log("Idle");
    }

    protected override void HandleStateTransition(PlayerContext Context ,float input)
    {
        if (Mathf.Abs(input) > 0.01f)
        {
            Context.StateMachine.ChangeState(ECharacterStateId.Walk);
        }
    }
}
