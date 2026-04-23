using Game.Application.Events;
using Game.Application.UI;
using Game.Application.UI.Core.Abstractions;
using UnityEngine;
namespace Game.Presentation.UI.View
{ 
    public class PlayerHUDPresenter : UIViewPresenter
    {
        public PlayerHUDViewModel ViewModel;

        public override void Bind(IUIView view,ViewModelBase viewModel)
        {
            ViewModel = (PlayerHUDViewModel)viewModel;
        }
    }
}
