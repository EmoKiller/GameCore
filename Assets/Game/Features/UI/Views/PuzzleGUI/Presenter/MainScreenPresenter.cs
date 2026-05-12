using Game.Application.Events;
using Game.Application.UI;
using Game.Application.UI.Commands;
using Game.Application.UI.Core.Abstractions;
using Game.Presentation.UI.Binding;
using UnityEngine;
namespace Game.Presentation.UI.View
{ 
    public class MainScreenPresenter : UIViewPresenter<MainScreen,MainScreenModel>
    {
        private BottomNavigation _bottomNavigation = new();
        protected override void OnBind()
        {
            _bottomNavigation.Bind(View.BottomNavigation,ViewModel.MainScreenBottomModel);
            AddDisposable(_bottomNavigation);   


            ViewModel.SetPlayCommand(new RelayCommand(StartLevel));
            AddDisposable(View.Buttonstart.Command(ViewModel.PlayLevelCommand));

        }

        private void StartLevel()
        {
            Debug.Log("Play Level");
        }
    }
}
