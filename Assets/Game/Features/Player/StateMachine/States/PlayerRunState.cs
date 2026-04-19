using UnityEngine;

public class PlayerRunState : PlayerLocomotionState
{
    protected override void OnEnter(PlayerContext Context)
    {
        Debug.Log("Run");
        Context.Core.Movement.SetSprint(true);
    }
    protected override void HandleStateTransition(PlayerContext Context, float input)
    {
        if (Context.CharacterInput.SprintHeld == false)
        {
            Context.Core.Movement.SetSprint(false);
            Context.StateMachine.ChangeState(ECharacterStateId.Walk);
        }

    }
}
