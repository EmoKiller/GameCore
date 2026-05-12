using Game.Application.ReactiveProperty;
using Game.Application.UI;
using UnityEngine;

public sealed class MainScreenBottomModel : IViewModel
{
    public ReactiveProperty<ENavigationTab> SelectedTab = new(ENavigationTab.Map);
}
