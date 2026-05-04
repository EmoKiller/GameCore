using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class SpawnPlayerMiddleware : ILoadingMiddleware
{
    private readonly IPlayerService _playerService;
    //private readonly Transform _parent;

    public SpawnPlayerMiddleware(IPlayerService playerService)//, Transform parent)
    {
        _playerService = playerService;
       // _parent = parent;
    }

    public async UniTask InvokeAsync(
        LoadingContext context,
        Func<UniTask> next,
        CancellationToken ct)
    {
        await next();

        // đảm bảo scene đã load xong
        _playerService.Spawn();
    }
}
