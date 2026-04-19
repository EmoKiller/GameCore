using System;
using System.Collections.Generic;
using Game.Application.UI;
using Game.Presentation.UI.Data;
using UI.Core.ViewModel;

public static class UIFactoryInstaller
{
    // public static void Install(
    //     IEnumerable<UIManifestEntry> entries,
    //     ViewModelFactory vmFactory,
    //     PresenterFactory presenterFactory)
    // {
    //     foreach (var entry in entries)
    //     {
    //         RegisterViewModel(entry, vmFactory);
    //         RegisterPresenter(entry, presenterFactory);
    //     }
    // }

    // private static void RegisterViewModel(
    //     UIManifestEntry entry,
    //     ViewModelFactory factory)
    // {
    //     factory.Register(() =>
    //         (ViewModelBase)Activator.CreateInstance(entry.ViewModelType));
    // }

    // private static void RegisterPresenter(
    //     UIManifestEntry entry,
    //     PresenterFactory factory)
    // {
    //     factory.Register(() =>
    //         (UIViewPresenter<ViewModelBase>)Activator.CreateInstance(entry.PresenterType));
    // }
}