using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Share.StateMachine;
using UnityEngine;

public sealed class MainMenuState : IAsyncState<RPGGameStateContext>
{
    public async UniTask EnterAsync(RPGGameStateContext ctx, CancellationToken ct)
    {
        await ctx.UIService.ShowAsync<MainMenuScreen>(ct);
    }

    public async UniTask ExitAsync(RPGGameStateContext ctx, CancellationToken ct)
    {
        await ctx.UIService.HideAsync<MainMenuScreen>(ct);
    }
}
