using Cysharp.Threading.Tasks;
using Game.Application.Events;

namespace Game.Presentation.Events
{
    public abstract class MonoAsyncEventHandler<TEvent> 
        : MonoEventBinder, IAsyncEventHandler<TEvent>
        where TEvent : IEvent
    {
        public abstract int Priority { get; }
        public abstract EventChannel Channel { get; }

        public abstract UniTask HandleAsync(TEvent evt);

        protected override void Bind()
        {
            EventBus.SubscribeAsync(this);
        }

        protected override void Unbind()
        {
            EventBus.UnsubscribeAsync(this);
        }
    }
}