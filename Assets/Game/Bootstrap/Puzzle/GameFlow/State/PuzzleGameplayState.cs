using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Share.StateMachine;
using UnityEngine;

public class PuzzleGameplayState : IAsyncState<PuzzleGameStateContext>
{
    
    public async UniTask EnterAsync(PuzzleGameStateContext context, CancellationToken ct)
    {
        await context.PuzzleGameplayService.CreateRuntime(ct);

        await UniTask.CompletedTask;
    }

    public async UniTask ExitAsync(PuzzleGameStateContext context, CancellationToken ct)
    {
        await UniTask.CompletedTask;
    }

    
}
