using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;


public sealed class InitializeBoard : ILoadingMiddleware
{
    
    public InitializeBoard()
    {
    }

    public async UniTask InvokeAsync(
        LoadingContext context,
        Func<UniTask> next,
        CancellationToken ct)
    {
        

        await next();
    }
}