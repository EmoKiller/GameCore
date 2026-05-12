using UnityEngine;
using UnityEngine.UI;
namespace Game.Presentation.UI.View
{ 
    public class MainScreen : UIView
    {
        [SerializeField] 
        private MainScreenBottomNavigation _bottomNavigation;
        public MainScreenBottomNavigation BottomNavigation => _bottomNavigation;


        [SerializeField] 
        private Button _buttonstart;
        public Button Buttonstart => _buttonstart;
    }
}