using Game.Application.Core;
using Game.Application.Events;
using Game.Application.UI;
using Game.Application.UI.Commands;
using UnityEngine;

public class MainMenuScreenViewModel : IViewModel 
{
    public RelayCommand PlayCommand { get; private set;}
    public RelayCommand OpenSettingsCommand { get; private set;}
    public RelayCommand SaveProgressCommand { get; private set;}

    public void MainScreenModelSetCommand(IMainScreenActions action)
    {
        PlayCommand = new RelayCommand(action.Play);
        OpenSettingsCommand = new RelayCommand(action.OpenSettings);
        SaveProgressCommand = new RelayCommand(action.SaveProgress);
    }

    
}
