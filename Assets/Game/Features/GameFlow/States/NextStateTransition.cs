using System;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Share.StateMachine;

public sealed class LoadingTransition<TContext> : 
    IAsyncTransition<EGameState, TContext>
    where TContext : class, ILoadingContext
{
    public EGameState From => EGameState.Loading;

    public EGameState To => throw new NotSupportedException(
        "Use GetTargetState instead");

    public UniTask<bool> CanTransitionAsync(TContext context, CancellationToken ct)
    {
        return UniTask.FromResult(context.IsLoadingCompleted);
    }

    public EGameState GetNextState(TContext context)
    {
        return context.NextGameState;
    }
}