using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Share.StateMachine;
using UnityEngine;

public sealed class GameplayState : IAsyncState<GameStateContext>
{
    public async UniTask EnterAsync(GameStateContext ctx, CancellationToken ct)
    {
        Debug.Log("Enter GamePlay");
        
        ctx.CameraService.SetFollowTarget(ctx.PlayerService.GetTransform());
        
        await UniTask.CompletedTask;
    }

    public async UniTask ExitAsync(GameStateContext ctx, CancellationToken ct)
    {
        Debug.Log("Exit GamePlay");
        await UniTask.CompletedTask;
    }
}
