using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Application.Loading.Abstractions
{
    public interface ILoadingOperation
    {
        EGameState TargetState {get;}
        UniTask ExecuteAsync(GameStateContext context, CancellationToken ct);
    }
    
    
}