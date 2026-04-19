using UnityEngine;
using Game.Application.UI.Core.Abstractions;
using UnityEngine.UI;
using Game.Presentation.UI;
using Game.Presentation.UI.Binding;

public class MainMenuScreen : UIView<MainMenuScreenViewModel>
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    protected override void OnBind(MainMenuScreenViewModel viewModel)
        {
            // View -> ViewModel (Command binding)
            viewModel.AddDisposable(playButton.Command(ViewModel.PlayCommand));
            viewModel.AddDisposable(settingsButton.Command(ViewModel.OpenSettingsCommand));
            viewModel.AddDisposable(quitButton.Command(ViewModel.QuitCommand));
        }
}