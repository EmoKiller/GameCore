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
    private readonly TransitionStateMachine<ECharacterStateId, PlayerContext> _stateMachine;
    public TransitionStateMachine<ECharacterStateId, PlayerContext> StateMachine => _stateMachine;

    public PlayerStateSystem(PlayerContext context)
    {
        _stateMachine = new TransitionStateMachine<ECharacterStateId, PlayerContext>(context);

        RegisterStates();
        RegisterTransitions();

        _stateMachine.ChangeState(ECharacterStateId.Idle);
    }
     private void RegisterStates()
    {
        _stateMachine.RegisterState(ECharacterStateId.Idle, new PlayerIdleState());
        _stateMachine.RegisterState(ECharacterStateId.Walk, new PlayerWalkState());
        _stateMachine.RegisterState(ECharacterStateId.Run, new PlayerRunState());
        _stateMachine.RegisterState(ECharacterStateId.Jump, new PlayerJumpState());
        _stateMachine.RegisterState(ECharacterStateId.Fall, new PlayerFallState());
    }
    private void RegisterTransitions()
    {
        // =========================
        // INTERRUPT (HIGH PRIORITY)
        // =========================

        _stateMachine.AddTransition(new JumpFromIdleTransition(), TransitionPhase.Interrupt);
        _stateMachine.AddTransition(new JumpFromWalkTransition(), TransitionPhase.Interrupt);
        _stateMachine.AddTransition(new JumpFromRunTransition(), TransitionPhase.Interrupt);


        // =========================
        // GROUND MOVEMENT
        // =========================

        // Idle <-> Walk
        _stateMachine.AddTransition(new IdleToWalkTransition());
        _stateMachine.AddTransition(new WalkToIdleTransition());

        // Walk <-> Run
        _stateMachine.AddTransition(new WalkToRunTransition());
        _stateMachine.AddTransition(new RunToWalkTransition());


        // =========================
        // AIR
        // =========================

        // Jump -> Fall
        _stateMachine.AddTransition(new JumpToFallTransition());

        // Fall -> Ground
        _stateMachine.AddTransition(new FallToIdleTransition());
        _stateMachine.AddTransition(new FallToWalkTransition());


        // =========================
        // SAFETY (RECOMMENDED)
        // =========================

        // Idle fallback (tránh stuck state)
        _stateMachine.AddTransition(new IdleFallbackTransition(), TransitionPhase.Fallback);
    }
    public void Tick(float dt)
    {
        _stateMachine.Update(dt);
        _stateMachine.Evaluate();
    }

    public void ChangeState(ECharacterStateId state)
    {
        _stateMachine.ChangeState(state);
    }
}
