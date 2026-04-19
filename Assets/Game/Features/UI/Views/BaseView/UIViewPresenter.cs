using System;
using Game.Application.Events;
using Game.Application.UI;
using Game.Shared.Lifecycle;

public abstract class UIViewPresenter : DisposableObject 
{
    
    public abstract int Priority { get; }
    public abstract EventChannel Channel { get; }
    public abstract void Bind(ViewModelBase viewModel);
}