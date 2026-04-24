using Game.Presentation.UI;
using Game.Presentation.UI.Binding;
using UnityEngine;
using UnityEngine.UI;
namespace Game.Presentation.UI.View
{ 
    public class PlayerHUDView : UIView
    {
        [SerializeField] private Slider hPBar;
        public Slider HPBar => hPBar;
        [SerializeField] private Slider manaBar;
        public Slider ManaBar => manaBar;
        [SerializeField] private Slider staminaBar;
        public Slider StaminaBar => staminaBar;
        
    }
}
