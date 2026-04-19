using Game.Application.Core;
using Game.Application.Events;
using Game.Application.UI;
using Game.Application.UI.Commands;
using UnityEngine;

public class MainMenuScreenViewModel : ViewModelBase 
{
    public RelayCommand PlayCommand { get; }
    public RelayCommand OpenSettingsCommand { get; }
    public RelayCommand QuitCommand { get; }

    public MainMenuScreenViewModel()
    {
        PlayCommand = new RelayCommand(Play);
        OpenSettingsCommand = new RelayCommand(OpenSettings);
        QuitCommand = new RelayCommand(Quit);
    }

    private void Play()
    {
        Debug.Log("Play");
        GameApplication.Instance.Services.Resolve<IEventBus>().Publish(new RequestStateChangeEvent(EGameState.Gameplay),EventChannel.System);
    }

    private void OpenSettings()
    {
        Debug.Log("Setting");
    }

    private void Quit()
    {
        UnityEngine.Application.Quit();
    }
}
