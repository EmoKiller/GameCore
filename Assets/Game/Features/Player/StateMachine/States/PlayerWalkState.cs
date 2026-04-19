using UnityEngine;

public class PlayerWalkState : PlayerLocomotionState
{
    protected override void OnEnter(PlayerContext Context)
    {
        Debug.Log("Walk");
        Context.Core.Movement.SetSprint(false);
    }

    protected override void HandleStateTransition(PlayerContext Context,float input)
    {
        if (Context.CharacterInput.SprintHeld == true)
        {
            Context.StateMachine.ChangeState(ECharacterStateId.Run);
            return;
        }

        if (Mathf.Abs(input) <= 0.01f)
        {
            Context.StateMachine.ChangeState(ECharacterStateId.Idle);
        }
    }
}
