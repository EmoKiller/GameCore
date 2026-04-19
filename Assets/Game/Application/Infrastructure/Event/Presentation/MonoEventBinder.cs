using Game.Application.Core;
using Game.Application.Events;
using UnityEngine;

namespace Game.Presentation.Events
{
    public abstract class MonoEventBinder : MonoBehaviour
    {
        protected IEventBus EventBus;

        protected virtual void Awake()
        {
            EventBus = ResolveEventBus();
        }

        protected virtual void OnEnable()
        {
            Bind();
        }

        protected virtual void OnDisable()
        {
            Unbind();
        }

        protected abstract void Bind();
        protected abstract void Unbind();

        private IEventBus ResolveEventBus()
        {
            // Bạn có thể thay bằng ServiceLocator hoặc DI
            return GameApplication.Instance.Services.Resolve<IEventBus>();
        }
    }
}