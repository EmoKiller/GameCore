using System;
using System.Collections.Generic;
using UnityEngine;
public interface IUINavigationGuardResolver
{
    IUINavigationGuard Resolve(
        Type from,
        Type to,
        string action);
}
public sealed class NavigationGuardResolver : IUINavigationGuardResolver
{
    private readonly Dictionary<string, IUINavigationGuard> _map = new();

    public void Register(string action, IUINavigationGuard guard)
    {
        _map[action] = guard;
    }

    public IUINavigationGuard Resolve(Type from, Type to, string action)
    {
        return _map.TryGetValue(action, out var guard)
            ? guard
            : null;
    }
}