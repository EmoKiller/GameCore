using System;
using System.Collections.Generic;
using Game.Shared.Lifecycle;
public enum UILifetime
{
    Global,
    Screen,
    Popup,
    Overlay
}
public sealed class UILifetimeScope : DisposableObject
{
    public UILifetime Lifetime { get; }

    public UILifetimeScope(
        UILifetime lifetime)
    {
        Lifetime = lifetime;
    }

    
    
}
