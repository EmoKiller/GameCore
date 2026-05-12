using Game.Presentation.UI.Binding;
using UnityEngine;

public class BottomNavigation : UIViewPresenter<MainScreenBottomNavigation,MainScreenBottomModel>
{
    protected override void OnBind()
    {
        AddDisposable(View.MapToggle.BindEnum(ViewModel.SelectedTab, ENavigationTab.Map)); 
        AddDisposable(View.EventsToggle.BindEnum(ViewModel.SelectedTab, ENavigationTab.Events)); 
        AddDisposable(View.ShopToggle.BindEnum(ViewModel.SelectedTab, ENavigationTab.Shop));
        
        AddDisposable(ViewModel.SelectedTab.Subscribe(OnTabChanged));

    }
    private void OnTabChanged(ENavigationTab tab)
    {
        switch (tab)
        {
            case ENavigationTab.Map:
                OpenMap();
                break;

            case ENavigationTab.Events:
                OpenEvents();
                break;

            case ENavigationTab.Shop:
                OpenShop();
                break;
        }
    }
    private void OpenMap()
    {
        Debug.Log("Open Map");
    }

    private void OpenEvents()
    {
        Debug.Log("Open Events");
    }

    private void OpenShop()
    {
        Debug.Log("Open Shop");
    }
}
