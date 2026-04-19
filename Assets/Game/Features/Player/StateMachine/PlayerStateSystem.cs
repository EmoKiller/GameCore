using Game.Share.StateMachine;
using UnityEngine;
public enum ECharacterStateId
{
    Idle,
    Walk,
    Run,
    Jump,
    Fall
}

public sealed class PlayerStateSystem
{
    private readonly StateMachine<ECharacterStateId, PlayerContext> _stateMachine;
    public StateMachine<ECharacterStateId, PlayerContext> StateMachine => _stateMachine;

    public PlayerStateSystem(PlayerContext context)
    {
        _stateMachine = new StateMachine<ECharacterStateId, PlayerContext>(context);

        _stateMachine.RegisterState(ECharacterStateId.Idle ,new PlayerIdleState());
        _stateMachine.RegisterState(ECharacterStateId.Walk ,new PlayerWalkState());
        _stateMachine.RegisterState(ECharacterStateId.Run ,new PlayerRunState());
        _stateMachine.RegisterState(ECharacterStateId.Jump ,new PlayerJumpState());
        _stateMachine.RegisterState(ECharacterStateId.Fall ,new PlayerFallState());
        
        _stateMachine.ChangeState(ECharacterStateId.Idle);
    }

    public void Tick(float dt)
    {
        _stateMachine.Update(dt);
    }

    public void ChangeState(ECharacterStateId state)
    {
        _stateMachine.ChangeState(state);
    }
}
