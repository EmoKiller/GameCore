using System;
using Game.Application.UI;
using Game.Application.UI.Core.Abstractions;

public sealed class UIHandle 
{
    public UIInstance Instance { get; }
    public UILifetimeScope Lifetime { get; }
    public UIHandleState State { get; set; }

    public UIHandle(UIInstance instance, UILifetimeScope lifetime)
    {
        Instance = instance;
        Lifetime = lifetime;
        State = UIHandleState.Active;
    }

    
}