using Game.Application.Core;
using Game.Application.Events;
using Game.Application.UI;
using Game.Application.UI.Commands;
using Game.Application.UI.Core.Abstractions;
using Game.Presentation.UI.Binding;
using UnityEngine;

public class MainMenuScreenPresenter : UIViewPresenter<MainMenuScreen,MainMenuScreenViewModel>
{
    protected override void OnBind()
    {

        ViewModel.SetPlayCommand(new RelayCommand(Play)); 
        ViewModel.SetOpenSettingsCommand(new RelayCommand(OpenSettings)); 
        ViewModel.SetQuitCommand(new RelayCommand(Quit));

        AddDisposable(View.PlayButton.Command(ViewModel.PlayCommand));
        AddDisposable(View.SettingsButton.Command(ViewModel.OpenSettingsCommand));
        AddDisposable(View.QuitButton.Command(ViewModel.QuitCommand));
        
    }

    private void Play()
    {
        //Debug.Log("Play");
        GameApplication.Instance.Services.Resolve<IEventBus>().Publish(new RequestStateChangeEvent(EGameState.Gameplay),EventChannel.System);
    }

    private void OpenSettings()
    {
        //Debug.Log("Setting");
    }

    private void Quit()
    {
        UnityEngine.Application.Quit();
    }
}
