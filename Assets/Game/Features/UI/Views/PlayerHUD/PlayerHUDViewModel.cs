using Game.Application.ReactiveProperty;
using Game.Application.UI;
using UnityEngine;
namespace Game.Presentation.UI.View
{ 
    public class PlayerHUDViewModel : IViewModel
    {
        public ReactiveProperty<float> HP { get; } = new(0f);
        public ReactiveProperty<float> Stamina { get; } = new(0f);
        public ReactiveProperty<float> Mana { get; } = new(0f);

    }
}
