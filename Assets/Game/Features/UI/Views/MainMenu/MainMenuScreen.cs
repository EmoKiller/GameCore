using UnityEngine;
using Game.Application.UI.Core.Abstractions;
using UnityEngine.UI;
using Game.Presentation.UI;
using Game.Presentation.UI.Binding;

public class MainMenuScreen : UIView 
{
    [SerializeField] private Button playButton;
    public Button PlayButton => playButton;
    [SerializeField] private Button settingsButton;
    public Button SettingsButton => settingsButton;
    [SerializeField] private Button quitButton;
    public Button QuitButton => quitButton;

}