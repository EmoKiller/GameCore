using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Application.Loading.Abstractions
{
    public interface ILoadingOperation<TContext>
    {
        EGameState TargetState { get; }

        UniTask ExecuteAsync(TContext context, CancellationToken ct);
    }
    
    
}