
using Game.Application.ReactiveProperty;
using Game.Application.UI;

namespace Game.Presentation.UI.View
{
    public sealed class LoadingViewModel : IViewModel 
    {
        public ReactiveProperty<float> Progress { get; } = new(0f);

        public IReadOnlyReactiveProperty<string> ProgressText { get; } 

        public LoadingViewModel()
        {
            
            ProgressText = Progress.Select(p => $"{(int)(p * 100)}%");


        }

    }
}