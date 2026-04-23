using Game.Application.UI;
using Game.Application.UI.Core.Abstractions;

public sealed class UIInstance
{
    public IUIView View { get; }
    public IViewModel ViewModel { get; }
    public UIViewPresenter Presenter { get; }

    public UIInstance(
        IUIView view,
        IViewModel viewModel,
        UIViewPresenter presenter)
    {
        View = view;
        ViewModel = viewModel;
        Presenter = presenter;
    }
}