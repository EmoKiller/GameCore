using Game.Application.Core;
using Game.Application.Events;
using Game.Application.UI;
using Game.Application.UI.Commands;
using UnityEngine;

public class MainMenuScreenViewModel : ViewModelBase 
{
    public RelayCommand PlayCommand { get; private set;}
    public RelayCommand OpenSettingsCommand { get; private set;}
    public RelayCommand QuitCommand { get; private set;}

    public void SetPlayCommand(RelayCommand playCommand)
    {
        PlayCommand = playCommand;
    }
    public void SetOpenSettingsCommand(RelayCommand openSettingsCommand)
    {
        OpenSettingsCommand = openSettingsCommand;
    }
    public void SetQuitCommand(RelayCommand quitCommand)
    {
        QuitCommand = quitCommand;
    }

    
}
