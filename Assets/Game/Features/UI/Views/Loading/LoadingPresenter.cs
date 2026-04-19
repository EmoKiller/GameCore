
using Game.Application.Core;
using Game.Application.Events;
using Game.Application.UI;
namespace Game.Presentation.UI.View
{
    public sealed class LoadingPresenter : UIViewPresenter ,
        IEventHandler<LoadingProgressEvent>
    {
        public LoadingViewModel ViewModel;
        public override int Priority => EventPriority.Normal;
        public override EventChannel Channel => EventChannel.UI;

        public override void Bind(ViewModelBase viewModel)
        {
            ViewModel = (LoadingViewModel)viewModel;

            GameApplication.Instance.Services.Resolve<IEventBus>().Subscribe(this);
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