using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Share.StateMachine;
using UnityEngine;

public sealed class BootState : IAsyncState<RPGGameStateContext>
{
    public async UniTask EnterAsync(RPGGameStateContext ctx, CancellationToken ct)
    {
        await UniTask.CompletedTask;
    }

    public async UniTask ExitAsync(RPGGameStateContext ctx, CancellationToken ct)
    {
        await UniTask.CompletedTask;
    }
    
       
}