using System;
using System.Collections.Generic;
using Game.Application.UI;
using Game.Application.UI.Core.Abstractions;
using Game.Presentation.UI.Data;
using UnityEngine;

public interface IPresenterFactory
{
    UIViewPresenter Create( Type presenterType, IUIView view, IViewModel viewModel);
}

public sealed class PresenterFactory : IPresenterFactory
{
    private readonly Dictionary<Type, Func<IUIView, IViewModel, UIViewPresenter>> _map
    = new();

    public void Register<TPresenter, TView, TViewModel>(
        Func<TView, TViewModel, TPresenter> factory)
        where TPresenter : UIViewPresenter
        where TView : class, IUIView
        where TViewModel : IViewModel
    {
        _map[typeof(TPresenter)] = (view, vm) =>
            factory((TView)view, (TViewModel)vm);
    }

    public UIViewPresenter Create(
        Type presenterType,
        IUIView view,
        IViewModel viewModel)
    {
        if (!_map.TryGetValue(presenterType, out var factory))
            throw new Exception($"Presenter not registered: {presenterType.Name}");

        return factory(view, viewModel);
    }
    public void RegisterDynamic(UIManifestEntry entry)
    {
        var presenterType = entry.PresenterType;
        var viewType = entry.ViewType;
        var vmType = entry.ViewModelType;

        _map[presenterType] = (view, vm) =>
        {
            var presenter = Activator.CreateInstance(presenterType);
            if (presenter is not UIViewPresenter)
            {
                Debug.Log("presenter is not UIViewPresenter<ViewModelBase>");
            }
            
            if (presenter is not UIViewPresenter p)
                throw new InvalidOperationException(
                    $"Invalid presenter: {presenterType.Name}");

            //p.Bind(view, vm);

            return p;
        };
    }

}