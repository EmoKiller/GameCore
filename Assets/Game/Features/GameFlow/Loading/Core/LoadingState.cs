using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Events;
using Game.Application.Loading.Abstractions;
using Game.Presentation.UI;
using Game.Presentation.UI.View;
using Game.Share.StateMachine;
using UnityEngine;

public sealed class LoadingState<TContext> : IAsyncState<TContext>
    where TContext : class, ILoadingStateContext
{
    private readonly Dictionary<EGameState, ILoadingOperation<TContext>> _tasks;

    public LoadingState(IEnumerable<ILoadingOperation<TContext>> tasks)
    {
        _tasks = tasks.ToDictionary(x => x.TargetState);
    }

    public async UniTask EnterAsync(TContext ctx, CancellationToken ct)
    {
        await ctx.ShowLoading(ct);

        if (_tasks.TryGetValue(ctx.NextGameState, out var strategy))
        {
            await strategy.ExecuteAsync(ctx, ct);
            ctx.MarkLoadingAsCompleted();
        }
        else
        {
            throw new Exception($"No loading strategy for {ctx.NextGameState}");
        }
    }

    public UniTask ExitAsync(TContext ctx, CancellationToken ct)
    {
        return ctx.HideLoading(ct);
    }
}
