using UnityEngine;
using UnityEngine.UI;
public class NavigationToggle : MonoBehaviour
{
    [SerializeField]
    private ENavigationTab _tab;

    [SerializeField]
    private Toggle _toggle;

    public ENavigationTab Tab => _tab;
    public Toggle Toggle => _toggle;
}