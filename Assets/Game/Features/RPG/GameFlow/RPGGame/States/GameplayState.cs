using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Presentation.UI.View;
using Game.Share.StateMachine;
using UnityEngine;

public sealed class GameplayState : IAsyncState<RPGGameStateContext>
{
    public async UniTask EnterAsync(RPGGameStateContext ctx, CancellationToken ct)
    {
        //Debug.Log("Enter GamePlay");
        
        ctx.CameraService.SetFollowTarget(ctx.PlayerService.GetTransform());
        await ctx.UIService.ShowAsync<PlayerHUDView>(ct);
        
        await UniTask.CompletedTask;
    }

    public async UniTask ExitAsync(RPGGameStateContext ctx, CancellationToken ct)
    {
        //Debug.Log("Exit GamePlay");
        await ctx.UIService.HideAsync<PlayerHUDView>(ct);
        await UniTask.CompletedTask;
    }
}
