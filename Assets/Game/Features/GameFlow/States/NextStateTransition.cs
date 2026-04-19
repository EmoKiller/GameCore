using System;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Share.StateMachine;

public sealed class LoadingTransition : IAsyncTransition<EGameState, GameStateContext>
{
    public EGameState From => EGameState.Loading;

    public EGameState To => throw new NotSupportedException(
        "Use GetTargetState instead");

    public UniTask<bool> CanTransitionAsync(GameStateContext context, CancellationToken ct)
    {
        return UniTask.FromResult(context.IsLoadingCompleted);
    }

    public EGameState GetNextState(GameStateContext context)
    {
        return context.NextGameState;
    }
}