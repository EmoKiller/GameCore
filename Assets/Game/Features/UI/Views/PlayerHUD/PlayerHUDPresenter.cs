using Game.Application.Events;
using Game.Application.UI;
using UnityEngine;
namespace Game.Presentation.UI.View
{ 
    public class PlayerHUDPresenter : UIViewPresenter
    {
        public PlayerHUDViewModel ViewModel;
        public override int Priority => EventPriority.Normal;

        public override EventChannel Channel => EventChannel.UI;

        public override void Bind(ViewModelBase viewModel)
        {
            ViewModel = (PlayerHUDViewModel)viewModel;
        }
    }
}
