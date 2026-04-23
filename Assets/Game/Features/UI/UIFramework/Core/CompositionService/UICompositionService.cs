using System;
using Game.Application.UI;
using Game.Application.UI.Core.Abstractions;
using UI.Core.ViewModel;
using UnityEngine;
public interface IUICompositionService
{
    UIInstance Compose( Type viewType, IUIView view, UILifetimeScope lifetime);
}
public sealed class UICompositionService : IUICompositionService
{
    private readonly UIManifest _manifest;
    private readonly IViewModelFactory _vmFactory;
    private readonly IPresenterFactory _presenterFactory;

    public UICompositionService(
        UIManifest manifest,
        IViewModelFactory vmFactory,
        IPresenterFactory presenterFactory)
    {
        _manifest = manifest;
        _vmFactory = vmFactory;
        _presenterFactory = presenterFactory;
    }

    public UIInstance Compose(
        Type viewType,
        IUIView view,
        UILifetimeScope lifetime)
    {
        var entry = _manifest.Get(viewType);
        var vm = CreateViewModel(entry.ViewModelType);
        var presenter = CreatePresenter(entry.PresenterType, view, vm);
        if (presenter != null)
        {
            presenter.Bind(view,vm);

            if (presenter is IDisposable d)
                lifetime.AddDisposable(d);
        }

        return new UIInstance(view, vm, presenter);
    }

    private IViewModel CreateViewModel(Type type)
    {
        if (type == null) return null;
        return _vmFactory.Create(type);
    }

    private UIViewPresenter CreatePresenter(Type type , IUIView view, IViewModel viewModel)
    {
        if (type == null) 
            return null;
        return _presenterFactory.Create(type,view,viewModel);
    }

}
