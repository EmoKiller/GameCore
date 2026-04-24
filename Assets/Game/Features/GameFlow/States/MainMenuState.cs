using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Share.StateMachine;
using UnityEngine;

public sealed class MainMenuState : IAsyncState<GameStateContext>
{
    public async UniTask EnterAsync(GameStateContext ctx, CancellationToken ct)
    {
        await ctx.UIService.ShowAsync<MainMenuScreen>(ct);
    }

    public async UniTask ExitAsync(GameStateContext ctx, CancellationToken ct)
    {
        await ctx.UIService.HideAsync<MainMenuScreen>(ct);
    }
}
