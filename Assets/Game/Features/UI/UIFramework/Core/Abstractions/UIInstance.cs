using Game.Application.UI;
using Game.Application.UI.Core.Abstractions;

public sealed class UIInstance
{
    public IUIView View { get; }
    public ViewModelBase ViewModel { get; }
    public UIViewPresenter Presenter { get; }

    public UIInstance(
        IUIView view,
        ViewModelBase viewModel,
        UIViewPresenter presenter)
    {
        View = view;
        ViewModel = viewModel;
        Presenter = presenter;
    }
}