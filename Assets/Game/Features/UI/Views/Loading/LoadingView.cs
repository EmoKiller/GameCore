using UnityEngine;
using UnityEngine.UI;
using Game.Presentation.UI.Binding;

namespace Game.Presentation.UI.View
{   
    public sealed class LoadingView : UIView 
    {
        [SerializeField] private Slider progressBar;
        public Slider ProgressBar => progressBar;
        [SerializeField] private TMPro.TextMeshProUGUI progressText;
        public TMPro.TextMeshProUGUI ProgressText => progressText;

    }
}