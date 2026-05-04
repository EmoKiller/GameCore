using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ILoadingMiddleware
{
    UniTask InvokeAsync(LoadingContext ctx, Func<UniTask> next, CancellationToken ct);
}
public interface IWeightedLoadingMiddleware : ILoadingMiddleware
{
    float Weight { get; }
}