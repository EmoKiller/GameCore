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
        ViewModel.SetQuitCommand(new RelayCommand(SaveProgress));

        AddDisposable(View.PlayButton.Command(ViewModel.PlayCommand));
        AddDisposable(View.SettingsButton.Command(ViewModel.OpenSettingsCommand));
        AddDisposable(View.ButtonSaveProgress.Command(ViewModel.SaveProgressCommand));
        
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

    private void SaveProgress()
    {
        UnityEngine.Application.Quit();
    }
}
