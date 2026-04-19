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

public class LoadingState : IAsyncState<GameStateContext>
{
    private readonly Dictionary<EGameState, ILoadingOperation> _LoadingTask;

    public LoadingState(IEnumerable<ILoadingOperation> task)
    {
         _LoadingTask = task.ToDictionary(x => x.TargetState);
    }
    public async UniTask EnterAsync(GameStateContext ctx, CancellationToken ct)
    {

        Debug.Log("Enter Loading");
        Debug.Log("Next State " + ctx.NextGameState);

        await ctx.UIService.ShowAsync<LoadingView>(ct);
        
        if (_LoadingTask.TryGetValue(ctx.NextGameState, out var strategy))
        {
            Debug.Log("Start Loading");    

            await strategy.ExecuteAsync(ctx,ct);

            Debug.Log("Loading Done");

            ctx.MarkLoadingAsCompleted();

        }
        else
        {
            throw new Exception($"No loading strategy for {ctx.NextGameState}");
        }    
    }

    public async UniTask ExitAsync(GameStateContext ctx, CancellationToken ct)
    {
        Debug.Log("Exit Loading");
        await ctx.UIService.HideAsync<LoadingView>(ct);
    }

}
