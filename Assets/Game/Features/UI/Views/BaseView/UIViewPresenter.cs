using System;
using Game.Application.Events;
using Game.Application.UI;
using Game.Application.UI.Core.Abstractions;
using Game.Presentation.UI;
using Game.Shared.Lifecycle;

public abstract class UIViewPresenter : DisposableObject 
{
    public abstract void Bind(IUIView view,ViewModelBase viewModel);
}