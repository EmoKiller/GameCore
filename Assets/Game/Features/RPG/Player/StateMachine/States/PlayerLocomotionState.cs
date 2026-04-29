using Game.Character.Core.Stats;
using UnityEngine;

public abstract class PlayerLocomotionState : CharacterState<PlayerContext>
{
    protected override void OnUpdate(PlayerContext context ,float dt)
    {
        // Movement
        //var speed = context.Stats.GetStat(EStatType.MoveSpeed).FinalValue;
        context.Actions.Move(context.Input.MoveInput);

        // Animation
        context.Actions.SetAnimatorSpeed();
        
        // Facing
        context.FlipCharacter.HandleFacing(context.Input.MoveInput);

    }

}
