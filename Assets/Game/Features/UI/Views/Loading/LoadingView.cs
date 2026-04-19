using UnityEngine;
using UnityEngine.UI;
using Game.Presentation.UI.Binding;

namespace Game.Presentation.UI.View
{   
    public sealed class LoadingView : UIView<LoadingViewModel>
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Slider progressBar;
        [SerializeField] private TMPro.TextMeshProUGUI progressText;


        protected override void OnBind(LoadingViewModel viewModel)
        {
            AddBinding(progressBar.To(ViewModel.Progress));
            AddBinding(progressText.To(ViewModel.ProgressText));
        }
    }
}