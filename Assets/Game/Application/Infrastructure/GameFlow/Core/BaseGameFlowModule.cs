using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using Game.Application.Events;
using Game.Share.StateMachine;

public abstract class BaseGameFlowModule <TContext> 
    : BaseGameModule, IEventHandler<RequestStateChangeEvent>
    where TContext : class, IGameFlowContext
{
    protected AsyncStateMachine<EGameState, TContext> _stateMachine;
    private CancellationTokenSource _stateCts;

    public int Priority => EventPriority.Normal;
    public EventChannel Channel => EventChannel.System;

    protected override async UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct)
    {
        services.Resolve<IEventBus>().Subscribe(this);

        var context = CreateContext(services);
        _stateMachine = new AsyncStateMachine<EGameState, TContext>(context);

        RegisterStates(_stateMachine);
        RegisterLoadingState(_stateMachine);
        RegisterTransitions(_stateMachine);

        await _stateMachine.SetInitialStateAsync(GetInitialState(), ct);
        await ChangeStateAsync(GetStartState(), ct);
    }

    // ===== EXTENSION POINTS =====

    protected abstract TContext CreateContext(IServiceContainer services);

    protected abstract void RegisterStates(
        AsyncStateMachine<EGameState, TContext> stateMachine);

    protected abstract void RegisterLoadingState(
        AsyncStateMachine<EGameState, TContext> stateMachine);

    protected abstract void RegisterTransitions(
        AsyncStateMachine<EGameState, TContext> stateMachine);

    protected abstract EGameState GetInitialState();

    protected abstract EGameState GetStartState();


    public async UniTask ChangeStateAsync(EGameState state, CancellationToken ct)
    {
        _stateMachine.Context.SetNextState(state);
        await _stateMachine.ChangeStateAsync(EGameState.Loading, ct);
        await _stateMachine.TryTransitionAsync(ct);
    }

    public async void Handle(RequestStateChangeEvent evt)
    {
        ResetStateCancellation();

        try
        {
            await ChangeStateAsync(evt.Target, _stateCts.Token);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void ResetStateCancellation()
    {
        _stateCts?.Cancel();
        _stateCts?.Dispose();
        _stateCts = new CancellationTokenSource();
    }
}
