using Game.Presentation.UI;
using Game.Presentation.UI.Binding;
using UnityEngine;
using UnityEngine.UI;
namespace Game.Presentation.UI.View
{ 
    public class PlayerHUDView : UIView
    {
        [SerializeField] private Slider HPBar;
        [SerializeField] private Slider ManaBar;
        [SerializeField] private Slider StaminaBar;
        // protected override void OnBind(PlayerHUDViewModel viewModel)
        // {
        //     AddBinding(HPBar.To(ViewModel.HP));
        //     AddBinding(ManaBar.To(ViewModel.Mana));
        //     AddBinding(StaminaBar.To(ViewModel.Stamina));
        // }
    }
}
