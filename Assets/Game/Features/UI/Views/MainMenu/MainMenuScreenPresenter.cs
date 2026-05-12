using Game.Application.Core;
using Game.Application.Events;
using Game.Presentation.UI.Binding;
using UnityEngine;

public class MainMenuScreenPresenter : UIViewPresenter<MainMenuScreen,MainMenuScreenViewModel> , IMainScreenActions
{
    protected override void OnBind()
    {

        ViewModel.MainScreenModelSetCommand(this);

        AddDisposable(View.PlayButton.Command(ViewModel.PlayCommand));
        AddDisposable(View.SettingsButton.Command(ViewModel.OpenSettingsCommand));
        AddDisposable(View.ButtonSaveProgress.Command(ViewModel.SaveProgressCommand));
        
    }

    public void Play()
    {
        //Debug.Log("Play");
        GameApplication.Instance.Services.Resolve<IEventBus>().Publish(new RequestStateChangeEvent(EGameState.Gameplay),EventChannel.System);
    }

    public void OpenSettings()
    {
        //Debug.Log("Setting");
    }

    public void SaveProgress()
    {
        
    }

}
