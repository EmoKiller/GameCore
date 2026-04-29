using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Share.StateMachine;
using UnityEngine;

public class PuzzleMainMenuState : IAsyncState<PuzzleGameStateContext>
{
    public async UniTask EnterAsync(PuzzleGameStateContext context, CancellationToken ct)
    {
        await context.UIService.ShowAsync<MainMenuScreen>(ct);
    }

    public async UniTask ExitAsync(PuzzleGameStateContext context, CancellationToken ct)
    {
        await context.UIService.HideAsync<MainMenuScreen>(ct);
    }
}
