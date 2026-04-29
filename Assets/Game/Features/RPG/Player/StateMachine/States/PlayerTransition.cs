using Game.Share.StateMachine;
using UnityEngine;

public sealed class IdleToWalkTransition : ITransition<ECharacterStateId, PlayerContext>
{
    public ECharacterStateId From => ECharacterStateId.Idle;
    public ECharacterStateId To => ECharacterStateId.Walk;

    public bool CanTransition(PlayerContext ctx)
    {
        return Mathf.Abs(ctx.Input.MoveInput.x) > 0.01f;
    }
}
public sealed class WalkToIdleTransition : ITransition<ECharacterStateId, PlayerContext>
{
    public ECharacterStateId From => ECharacterStateId.Walk;
    public ECharacterStateId To => ECharacterStateId.Idle;

    public bool CanTransition(PlayerContext ctx)
    {
        return Mathf.Abs(ctx.Input.MoveInput.x) <= 0.01f;
    }
}
public sealed class WalkToRunTransition  : ITransition<ECharacterStateId, PlayerContext>
{
    public ECharacterStateId From => ECharacterStateId.Walk;
    public ECharacterStateId To => ECharacterStateId.Run;

    public bool CanTransition(PlayerContext ctx)
    {
        return ctx.Input.SprintHeld;
    }
}
public sealed class RunToWalkTransition  : ITransition<ECharacterStateId, PlayerContext>
{
    public ECharacterStateId From => ECharacterStateId.Run;
    public ECharacterStateId To => ECharacterStateId.Walk;

    public bool CanTransition(PlayerContext ctx)
    {
        return !ctx.Input.SprintHeld;
    }
}
public sealed class JumpTransition  : ITransition<ECharacterStateId, PlayerContext>
{
    public ECharacterStateId From => ECharacterStateId.Idle; // sẽ register nhiều lần
    public ECharacterStateId To => ECharacterStateId.Jump;

    public bool CanTransition(PlayerContext ctx)
    {
        return ctx.Input.JumpHeld;
    }
}
public sealed class JumpToFallTransition  : ITransition<ECharacterStateId, PlayerContext>
{
    public ECharacterStateId From => ECharacterStateId.Jump;
    public ECharacterStateId To => ECharacterStateId.Fall;

    public bool CanTransition(PlayerContext ctx)
    {
        return ctx.Actions.ActualVelocity().y <= 0f;
    }
}

public sealed class FallToIdleTransition  : ITransition<ECharacterStateId, PlayerContext>
{
    public ECharacterStateId From => ECharacterStateId.Fall;
    public ECharacterStateId To => ECharacterStateId.Idle;

    public bool CanTransition(PlayerContext ctx)
    {
        return ctx.Actions.IsGrouned() &&
               Mathf.Abs(ctx.Input.MoveInput.x) <= 0.01f;
    }
}
public sealed class FallToWalkTransition  : ITransition<ECharacterStateId, PlayerContext>
{
    public ECharacterStateId From => ECharacterStateId.Fall;
    public ECharacterStateId To => ECharacterStateId.Walk;

    public bool CanTransition(PlayerContext ctx)
    {
        return ctx.Actions.IsGrouned() &&
               Mathf.Abs(ctx.Input.MoveInput.x) > 0.01f;
    }
}

public sealed class IdleFallbackTransition 
    : ITransition<ECharacterStateId, PlayerContext>
{
    public ECharacterStateId From => ECharacterStateId.Idle;
    public ECharacterStateId To => ECharacterStateId.Idle;

    public bool CanTransition(PlayerContext ctx)
    {
        // luôn false → chỉ để debug safety
        return false;
    }
}

public sealed class JumpFromIdleTransition 
    : ITransition<ECharacterStateId, PlayerContext>
{
    public ECharacterStateId From => ECharacterStateId.Idle;
    public ECharacterStateId To => ECharacterStateId.Jump;

    public bool CanTransition(PlayerContext ctx)
        => ctx.Input.JumpHeld;
}

public sealed class JumpFromWalkTransition 
    : ITransition<ECharacterStateId, PlayerContext>
{
    public ECharacterStateId From => ECharacterStateId.Walk;
    public ECharacterStateId To => ECharacterStateId.Jump;

    public bool CanTransition(PlayerContext ctx)
        => ctx.Input.JumpHeld;
}

public sealed class JumpFromRunTransition 
    : ITransition<ECharacterStateId, PlayerContext>
{
    public ECharacterStateId From => ECharacterStateId.Run;
    public ECharacterStateId To => ECharacterStateId.Jump;

    public bool CanTransition(PlayerContext ctx)
        => ctx.Input.JumpHeld;
}