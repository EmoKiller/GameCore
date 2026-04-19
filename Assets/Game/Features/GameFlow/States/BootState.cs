using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Share.StateMachine;
using UnityEngine;

public sealed class BootState : IAsyncState<GameStateContext>
{
    public async UniTask EnterAsync(GameStateContext ctx, CancellationToken ct)
    {
        Debug.Log("Enter BootState");
        await UniTask.CompletedTask;
    }

    public async UniTask ExitAsync(GameStateContext ctx, CancellationToken ct)
    {
        Debug.Log("Exit BootState");
        await UniTask.CompletedTask;
    }
    
       
}