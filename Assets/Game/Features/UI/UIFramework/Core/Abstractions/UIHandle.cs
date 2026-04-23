using System;
using Game.Application.UI;
using Game.Application.UI.Core.Abstractions;

public sealed class UIHandle 
{
    public UIInstance Instance { get; }
    public UICompositionScope CompositionScope { get; }
    public UIHandleState State { get; set; }

    public UIHandle(UIInstance instance, UICompositionScope compositionScope)
    {
        Instance = instance;
        CompositionScope = compositionScope;
        State = UIHandleState.Active;
    }
    
}