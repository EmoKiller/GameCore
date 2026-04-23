using System;
using Game.Application.Events;
using Game.Application.UI;
using Game.Application.UI.Core.Abstractions;
using Game.Presentation.UI;
using Game.Shared.Lifecycle;

public abstract class UIViewPresenter : DisposableObject 
{
    public abstract void Bind(IUIView view,IViewModel viewModel);
}
public abstract class UIViewPresenter<TView, TViewModel> : UIViewPresenter
    where TView : class, IUIView
    where TViewModel : IViewModel
{
    protected TView View { get; private set; }
    protected TViewModel ViewModel { get; private set; }

    public sealed override void Bind(IUIView view, IViewModel viewModel)
    {
        if (view is not TView v)
            throw new InvalidOperationException(
                $"View type mismatch. Expected {typeof(TView).Name}, got {view.GetType().Name}");

        if (viewModel is not TViewModel vm)
            throw new InvalidOperationException(
                $"ViewModel type mismatch. Expected {typeof(TViewModel).Name}, got {viewModel.GetType().Name}");

        View = v;
        ViewModel = vm;

        OnBind();
    }

    protected abstract void OnBind();
}