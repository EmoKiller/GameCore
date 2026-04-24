using Game.Application.Events;
using Game.Application.UI;
using Game.Application.UI.Core.Abstractions;
using Game.Presentation.UI.Binding;
using UnityEngine;
namespace Game.Presentation.UI.View
{ 
    public class PlayerHUDPresenter : UIViewPresenter<PlayerHUDView,PlayerHUDViewModel>
    {

        protected override void OnBind()
        {
            AddDisposable(View.HPBar.To(ViewModel.HP));
            AddDisposable(View.ManaBar.To(ViewModel.Mana));
            AddDisposable(View.StaminaBar.To(ViewModel.Stamina));
        }
    }
}
