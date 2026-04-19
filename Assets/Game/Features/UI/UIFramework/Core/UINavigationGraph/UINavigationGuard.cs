using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
public interface IUINavigationGuard
{
    UniTask<bool> CanNavigateAsync(
        Type from,
        Type to,
        CancellationToken ct);
}
public sealed class FuncNavigationGuard : IUINavigationGuard
{
    private readonly Func<Type, Type, CancellationToken, UniTask<bool>> _fn;

    public FuncNavigationGuard(
        Func<Type, Type, CancellationToken, UniTask<bool>> fn)
    {
        _fn = fn;
    }

    public UniTask<bool> CanNavigateAsync(
        Type from,
        Type to,
        CancellationToken ct)
    {
        return _fn(from, to, ct);
    }
}
