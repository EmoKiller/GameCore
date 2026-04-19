using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Game.Application.Events.Reactive
{
    public interface IEventStream<T>
    {
        IDisposable Subscribe(Action<T> listener);
    }
    public static class EventStreamReactive
    {
        public static IEventStream<T> Where<T>(
            this IEventStream<T> source,
            Func<T, bool> predicate)
        {
            var stream = new EventStream<T>();

            source.Subscribe(value =>
            {
                if (predicate(value))
                    stream.Publish(value);
            });

            return stream;
        }

        public static IEventStream<TResult> Select<T, TResult>(
            this IEventStream<T> source,
            Func<T, TResult> selector)
        {
            var stream = new EventStream<TResult>();

            source.Subscribe(value =>
            {
                stream.Publish(selector(value));
            });

            return stream;
        }

        public static IEventStream<T> Throttle<T>(
            this IEventStream<T> source,
            TimeSpan delay)
        {
            var stream = new EventStream<T>();

            bool waiting = false;

            source.Subscribe(async value =>
            {
                if (waiting) return;

                waiting = true;
                stream.Publish(value);

                await UniTask.Delay(delay);
                waiting = false;
            });

            return stream;
        }

        public static IEventStream<T> Debounce<T>(
            this IEventStream<T> source,
            TimeSpan delay)
        {
            var stream = new EventStream<T>();

            int version = 0;

            source.Subscribe(async value =>
            {
                int current = ++version;

                await UniTask.Delay(delay);

                if (current == version)
                    stream.Publish(value);
            });

            return stream;
        }

        public static IEventStream<(T1, T2)> CombineLatest<T1, T2>(
            IEventStream<T1> s1,
            IEventStream<T2> s2)
        {
            var stream = new EventStream<(T1, T2)>();

            bool has1 = false, has2 = false;
            T1 v1 = default;
            T2 v2 = default;

            s1.Subscribe(x =>
            {
                v1 = x;
                has1 = true;

                if (has2)
                    stream.Publish((v1, v2));
            });

            s2.Subscribe(x =>
            {
                v2 = x;
                has2 = true;

                if (has1)
                    stream.Publish((v1, v2));
            });

            return stream;
        }
    }
    public interface IEventReactive
    {
        IEventStream<TEvent> Stream<TEvent>(EventChannel channel)
            where TEvent : IEvent;
    }
    public sealed class EventStream<T> : IEventStream<T>
    {
        private readonly List<Action<T>> _listeners = new(4);

        public IDisposable Subscribe(Action<T> listener)
        {
            _listeners.Add(listener);
            return new Subscription(this, listener);
        }

        public void Publish(T value)
        {
            for (int i = 0; i < _listeners.Count; i++)
            {
                _listeners[i].Invoke(value);
            }
        }

        private void Unsubscribe(Action<T> listener)
        {
            _listeners.Remove(listener);
        }

        private sealed class Subscription : IDisposable
        {
            private readonly EventStream<T> _stream;
            private readonly Action<T> _listener;

            public Subscription(EventStream<T> stream, Action<T> listener)
            {
                _stream = stream;
                _listener = listener;
            }

            public void Dispose()
            {
                _stream.Unsubscribe(_listener);
            }
        }
    }
    public sealed class EventStreamAdapter<TEvent> : IEventHandler<TEvent>
        where TEvent : IEvent
    {
        private readonly EventStream<TEvent> _stream;

        public int Priority => EventPriority.Low;
        public EventChannel Channel { get; }

        public EventStreamAdapter(EventStream<TEvent> stream, EventChannel channel)
        {
            _stream = stream;
            Channel = channel;
        }

        public void Handle(TEvent evt)
        {
            _stream.Publish(evt);
        }
    }
    public sealed class EventReactiveService : IEventReactive
    {
        private readonly IEventBus _bus;

        private readonly Dictionary<(Type, EventChannel), object> _streams = new();

        public EventReactiveService(IEventBus bus)
        {
            _bus = bus;
        }

        public IEventStream<TEvent> Stream<TEvent>(EventChannel channel)
            where TEvent : IEvent
        {
            var key = (typeof(TEvent), channel);

            if (_streams.TryGetValue(key, out var existing))
                return (IEventStream<TEvent>)existing;

            var stream = new EventStream<TEvent>();
            var adapter = new EventStreamAdapter<TEvent>(stream, channel);

            _bus.Subscribe(adapter);

            _streams[key] = stream;
            return stream;
        }
    }

    
}
//USAGE EXAMPLES

// _reactive
//     .Stream<ShowLoadingEvent>(EventChannel.UI)
//     .Throttle(TimeSpan.FromMilliseconds(100))
//     .Subscribe(evt =>
//     {
//         ShowLoadingUI(evt.IsVisible);
//     });

// _reactive
//     .Stream<FireEvent>(EventChannel.Input)
//     .Debounce(TimeSpan.FromMilliseconds(200))
//     .Subscribe(_ => Fire());

// var hpStream = _reactive.Stream<HpChangedEvent>(EventChannel.Gameplay);
// var manaStream = _reactive.Stream<ManaChangedEvent>(EventChannel.Gameplay);

// hpStream
//     .CombineLatest(manaStream)
//     .Subscribe(data =>
//     {
//         UpdateHUD(data.Item1, data.Item2);
//     });


//     public sealed class EventReactiveModule : IGameModule
//     {
//         public string ModuleName => "EventReactiveModule";
//         public int InitializationOrder => 20;

//         public void Initialize(IServiceContainer container)
//         {
//             var bus = container.Resolve<IEventBus>();
//             container.RegisterSingleton<IEventReactive>(new EventReactiveService(bus));
//         }

//         public void Shutdown() { }
//     }