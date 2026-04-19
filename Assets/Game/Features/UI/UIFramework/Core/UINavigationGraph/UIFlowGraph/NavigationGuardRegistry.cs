

using System.Collections.Generic;

public interface IUINavigationGuardRegistry
{
    void Register(string action, IUINavigationGuard guard);

    IUINavigationGuard Resolve(string action);
}

public sealed class NavigationGuardRegistry : IUINavigationGuardRegistry
{
    private readonly Dictionary<string, IUINavigationGuard> _guards = new();

    public void Register(string action, IUINavigationGuard guard)
    {
        _guards[action] = guard;
    }

    public IUINavigationGuard Resolve(string action)
    {
        return _guards.TryGetValue(action, out var guard)
            ? guard
            : null;
    }
}