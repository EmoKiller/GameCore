

using Game.Application.Events;

namespace Game.Presentation.Events
{
    public abstract class MonoEventHandler<TEvent> 
        : MonoEventBinder, IEventHandler<TEvent>
        where TEvent : IEvent
    {
        public abstract int Priority { get; }
        public abstract EventChannel Channel { get; }

        public abstract void Handle(TEvent evt);

        protected override void Bind()
        {
            EventBus.Subscribe(this);
        }

        protected override void Unbind()
        {
            EventBus.Unsubscribe(this);
        }
    }
}