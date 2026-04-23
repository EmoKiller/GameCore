using System.Collections;

using Game.Application.Core;
using Game.Application.Events;
using Game.Application.UI;
using Game.Application.UI.Core.Abstractions;
using Game.Presentation.UI.Binding;
namespace Game.Presentation.UI.View
{
    public sealed class LoadingPresenter : UIViewPresenter<LoadingView,LoadingViewModel> ,
        IEventHandler<LoadingProgressEvent>
    {
        public int Priority => EventPriority.Normal;
        public EventChannel Channel => EventChannel.UI;
        

        protected override void OnBind()
        {

            GameApplication.Instance.Services.Resolve<IEventBus>().Subscribe(this);

            AddDisposable(View.ProgressBar.To(ViewModel.Progress));
            AddDisposable(View.ProgressText.To(ViewModel.ProgressText));
        }

        public void Handle(LoadingProgressEvent evt)
        {
            var progress = MapProgress(evt);

            if (ViewModel.Progress.Value == progress)
                return;

            ViewModel.Progress.Value = progress;
        }

        private static float MapProgress(LoadingProgressEvent evt)
        {
            return evt.Progress;
        }
        public override void Dispose()
        {
            base.Dispose();
            GameApplication.Instance.Services.Resolve<IEventBus>().Unsubscribe(this);
        }
        
    }
}