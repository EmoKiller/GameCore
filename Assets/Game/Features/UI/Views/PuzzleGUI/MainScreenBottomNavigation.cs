using Game.Presentation.UI;
using UnityEngine;
using UnityEngine.UI;

public class MainScreenBottomNavigation : UIView
{
    [SerializeField] private Toggle mapToggle;

    public Toggle MapToggle => mapToggle;
    [SerializeField] private Toggle eventsToggle;

    public Toggle EventsToggle => eventsToggle;
    [SerializeField] private Toggle shopToggle;

    public Toggle ShopToggle => shopToggle;
}
