
using System;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using Game.Application.Events.Debugging;
namespace Game.Application.Events
{
    public interface IEvent { }
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        int Priority { get; }
        EventChannel Channel { get; }

        void Handle(TEvent evt);
    }
    public interface IAsyncEventHandler<in TEvent>
        where TEvent : IEvent
    {
        int Priority { get; }
        EventChannel Channel { get; }

        UniTask HandleAsync(TEvent evt);
    }

    public interface IEventBus : IService
    {
        void Subscribe<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : IEvent;

        void SubscribeAsync<TEvent>(IAsyncEventHandler<TEvent> handler)
            where TEvent : IEvent;

        void Unsubscribe<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : IEvent;

        void UnsubscribeAsync<TEvent>(IAsyncEventHandler<TEvent> handler)
            where TEvent : IEvent;

        void Publish<TEvent>(TEvent evt, EventChannel channel)
            where TEvent : IEvent;

        UniTask PublishAsync<TEvent>(TEvent evt, EventChannel channel)
            where TEvent : IEvent;
    }

    public interface IParallelEvent { }
    internal interface IEventHandlerWrapper
    {
        int Priority { get; }
        EventChannel Channel { get; }
        void Invoke(IEvent evt);
    }
    internal interface IAsyncEventHandlerWrapper
    {
        int Priority { get; }
        EventChannel Channel { get; }
        UniTask Invoke(IEvent evt);
    }
    public enum EventChannel
    {
        Gameplay,
        UI,
        Audio,
        Network,
        System,
        Input
    }

    public static class EventPriority
    {
        public const int Low = 0;
        public const int Normal = 100;
        public const int High = 200;
        public const int Critical = 300;
    }
    public sealed class EventContext
    {
        public IEvent Event { get; }
        public EventChannel Channel { get; }

        public bool IsCancelled { get; private set; }

        public EventContext(IEvent evt, EventChannel channel)
        {
            Event = evt;
            Channel = channel;
        }

        public void Cancel()
        {
            IsCancelled = true;
        }
    }
    public interface IEventValidator
    {
        bool Validate(IEvent evt);
    }
    public interface IEventMiddleware
    {
        UniTask InvokeAsync(EventContext context, EventExecutionDelegate next);
    }

    public delegate UniTask EventExecutionDelegate(EventContext context);

    public interface ICancellableEvent : IEvent
    {
        bool ShouldCancel();
    }
    
}