using Game.Application.UI;
using Game.Application.UI.Commands;
using UnityEngine;
namespace Game.Presentation.UI.View
{ 
    public class MainScreenModel : IViewModel
    {
        public MainScreenBottomModel MainScreenBottomModel = new();
        public RelayCommand PlayLevelCommand { get; private set;}
        public void SetPlayCommand(RelayCommand playLevelCommand)
        {
            PlayLevelCommand = playLevelCommand;
        }
    }
}