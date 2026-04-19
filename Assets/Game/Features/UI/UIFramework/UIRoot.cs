using Game.Application.UI.Core.Abstractions;
using UnityEngine;

namespace Game.Presentation.UI
{
    public interface IUIRoot
    {
        Transform GetRoot(EUILayer layer);
    }
    public sealed class UIRoot : MonoBehaviour, IUIRoot
    {
        [Header("Roots")]
        [SerializeField] private Transform _screenRoot;
        [SerializeField] private Transform _popupRoot;
        [SerializeField] private Transform _overlayRoot;

        [Header("Canvases")]
        [SerializeField] private Canvas _screenCanvas;
        [SerializeField] private Canvas _popupCanvas;
        [SerializeField] private Canvas _overlayCanvas;

        private const int ScreenOrder = 0;
        private const int PopupOrder = 100;
        private const int OverlayOrder = 200;

        private void Awake()
        {
            SetupCanvas(_screenCanvas, ScreenOrder);
            SetupCanvas(_popupCanvas, PopupOrder);
            SetupCanvas(_overlayCanvas, OverlayOrder);
        }

        private void SetupCanvas(Canvas canvas, int order)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = order;
        }

        public Transform GetRoot(EUILayer layer)
        {
            return layer switch
            {
                EUILayer.Screen => _screenRoot,
                EUILayer.Popup => _popupRoot,
                EUILayer.Overlay => _overlayRoot,
                _ => _screenRoot
            };
        }
    }
}