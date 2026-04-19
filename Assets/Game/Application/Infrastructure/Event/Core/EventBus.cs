using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using Game.Application.Events.Debugging;

namespace Game.Application.Events
{
    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<IEventHandlerWrapper>> _syncRaw = new();
        private readonly Dictionary<Type, List<IAsyncEventHandlerWrapper>> _asyncRaw = new();

        private readonly Dictionary<Type, Dictionary<EventChannel, IEventHandlerWrapper[]>> _syncCache = new();
        private readonly Dictionary<Type, Dictionary<EventChannel, IAsyncEventHandlerWrapper[]>> _asyncCache = new();


        private readonly List<IEventMiddleware> _middlewares = new();
        public void AddMiddleware(IEventMiddleware middleware)
        {
            _middlewares.Add(middleware);
        }
        
        private EventExecutionDelegate BuildPipeline(EventExecutionDelegate terminal)
        {
            EventExecutionDelegate current = terminal;

            for (int i = _middlewares.Count - 1; i >= 0; i--)
            {
                var middleware = _middlewares[i];
                var next = current;

                current = async (ctx) =>
                {
                    if (ctx.IsCancelled)
                        return;

                    await middleware.InvokeAsync(ctx, next);
                };
            }

            return current;
        }
        private async UniTask ExecutePipeline(EventContext context)
        {
            var pipeline = BuildPipeline(ExecuteHandlers);

            await pipeline(context);
        }
        private async UniTask ExecuteHandlers(EventContext ctx)
        {
            if (ctx.IsCancelled)
                return;

            
            var type = ctx.Event.GetType();

            var syncArray = GetSyncArray(type, ctx.Channel);
            var asyncArray = GetAsyncArray(type, ctx.Channel);


            
            //

            // sync
            for (int i = 0; i < syncArray.Length; i++)
            {
                syncArray[i].Invoke(ctx.Event);
            }

            if (ctx.IsCancelled)
                return;

            var isParallel = ctx.Event is IParallelEvent;

            if (!isParallel)
            {
                for (int i = 0; i < asyncArray.Length; i++)
                {
                    await asyncArray[i].Invoke(ctx.Event);
                }

                return;
            }

            var tasks = new UniTask[asyncArray.Length];

            for (int i = 0; i < asyncArray.Length; i++)
            {
                tasks[i] = asyncArray[i].Invoke(ctx.Event);
            }

            await UniTask.WhenAll(tasks);
        }
        #region Subscribe

        public void Subscribe<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : IEvent
        {
            var type = typeof(TEvent);

            if (!_syncRaw.TryGetValue(type, out var list))
            {
                list = new List<IEventHandlerWrapper>(4);
                _syncRaw[type] = list;
            }

            list.Add(new EventHandlerWrapper<TEvent>(handler));
            Invalidate(type);
        }

        public void SubscribeAsync<TEvent>(IAsyncEventHandler<TEvent> handler)
            where TEvent : IEvent
        {
            var type = typeof(TEvent);

            if (!_asyncRaw.TryGetValue(type, out var list))
            {
                list = new List<IAsyncEventHandlerWrapper>(4);
                _asyncRaw[type] = list;
            }

            list.Add(new AsyncEventHandlerWrapper<TEvent>(handler));
            Invalidate(type);
        }

        #endregion

        #region Unsubscribe (optional minimal)

        public void Unsubscribe<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : IEvent
        {
            // intentionally not optimized (rare operation)
            var type = typeof(TEvent);

            if (_syncRaw.TryGetValue(type, out var list))
            {
                list.RemoveAll(w => ((EventHandlerWrapper<TEvent>)w).EqualsHandler(handler));
                Invalidate(type);
            }
        }

        public void UnsubscribeAsync<TEvent>(IAsyncEventHandler<TEvent> handler)
            where TEvent : IEvent
        {
            var type = typeof(TEvent);

            if (_asyncRaw.TryGetValue(type, out var list))
            {
                list.RemoveAll(w => ((AsyncEventHandlerWrapper<TEvent>)w).EqualsHandler(handler));
                Invalidate(type);
            }
        }

        #endregion

        #region Publish Sync

        public void Publish<TEvent>(TEvent evt, EventChannel channel)
            where TEvent : IEvent
        {
            // var array = GetSyncArray(typeof(TEvent), channel);
            // for (int i = 0; i < array.Length; i++)
            // {
            //     array[i].Invoke(evt);
            // }
            PublishAsync(evt, channel).Forget();
        }

        #endregion

        #region Publish Async

        public UniTask PublishAsync<TEvent>(TEvent evt, EventChannel channel)
            where TEvent : IEvent
        {
            var context = new EventContext(evt, channel);

            return ExecutePipeline(context);
        }

        #endregion

        #region Cache Build

        private IEventHandlerWrapper[] GetSyncArray(Type type, EventChannel channel)
        {
            if (!_syncCache.TryGetValue(type, out var channelMap))
            {
                channelMap = BuildSync(type);
                _syncCache[type] = channelMap;
            }

            if (channelMap.TryGetValue(channel, out var arr))
                return arr;

            return Array.Empty<IEventHandlerWrapper>();
        }

        private IAsyncEventHandlerWrapper[] GetAsyncArray(Type type, EventChannel channel)
        {
            if (!_asyncCache.TryGetValue(type, out var channelMap))
            {
                channelMap = BuildAsync(type);
                _asyncCache[type] = channelMap;
            }

            if (channelMap.TryGetValue(channel, out var arr))
                return arr;

            return Array.Empty<IAsyncEventHandlerWrapper>();
        }

        private Dictionary<EventChannel, IEventHandlerWrapper[]> BuildSync(Type type)
        {
            var result = new Dictionary<EventChannel, IEventHandlerWrapper[]>();

            if (!_syncRaw.TryGetValue(type, out var list))
                return result;

            var temp = new Dictionary<EventChannel, List<IEventHandlerWrapper>>();

            for (int i = 0; i < list.Count; i++)
            {
                var h = list[i];

                if (!temp.TryGetValue(h.Channel, out var l))
                {
                    l = new List<IEventHandlerWrapper>(4);
                    temp[h.Channel] = l;
                }

                l.Add(h);
            }

            foreach (var pair in temp)
            {
                var arr = pair.Value.ToArray();
                Array.Sort(arr, CompareSync);
                result[pair.Key] = arr;
            }

            return result;
        }

        private Dictionary<EventChannel, IAsyncEventHandlerWrapper[]> BuildAsync(Type type)
        {
            var result = new Dictionary<EventChannel, IAsyncEventHandlerWrapper[]>();

            if (!_asyncRaw.TryGetValue(type, out var list))
                return result;

            var temp = new Dictionary<EventChannel, List<IAsyncEventHandlerWrapper>>();

            for (int i = 0; i < list.Count; i++)
            {
                var h = list[i];

                if (!temp.TryGetValue(h.Channel, out var l))
                {
                    l = new List<IAsyncEventHandlerWrapper>(4);
                    temp[h.Channel] = l;
                }

                l.Add(h);
            }

            foreach (var pair in temp)
            {
                var arr = pair.Value.ToArray();
                Array.Sort(arr, CompareAsync);
                result[pair.Key] = arr;
            }

            return result;
        }

        private static int CompareSync(IEventHandlerWrapper a, IEventHandlerWrapper b)
            => b.Priority.CompareTo(a.Priority);

        private static int CompareAsync(IAsyncEventHandlerWrapper a, IAsyncEventHandlerWrapper b)
            => b.Priority.CompareTo(a.Priority);

        #endregion

        #region Cache Invalidate

        private void Invalidate(Type type)
        {
            _syncCache.Remove(type);
            _asyncCache.Remove(type);
        }

        #endregion
    }

    internal sealed class AsyncEventHandlerWrapper<TEvent> : IAsyncEventHandlerWrapper
        where TEvent : IEvent
    {
        private readonly IAsyncEventHandler<TEvent> _handler;

        public int Priority => _handler.Priority;
        public EventChannel Channel => _handler.Channel;

        public AsyncEventHandlerWrapper(IAsyncEventHandler<TEvent> handler)
        {
            _handler = handler;
        }

        public UniTask Invoke(IEvent evt)
        {
            return _handler.HandleAsync((TEvent)evt);
        }
        public void Invoke(IEvent evt, EventTrace trace)
        {
            var sw = Stopwatch.StartNew();

            _handler.HandleAsync((TEvent)evt).Forget();

            sw.Stop();

            trace.Handlers.Add(new HandlerTrace
            {
                HandlerName = _handler.GetType().Name,
                Duration = sw.ElapsedMilliseconds
            });
        }
        public bool EqualsHandler(IAsyncEventHandler<TEvent> other)
        {
            return ReferenceEquals(_handler, other);
        }
    }

    internal sealed class EventHandlerWrapper<TEvent> : IEventHandlerWrapper
        where TEvent : IEvent
    {
        private readonly IEventHandler<TEvent> _handler;

        public int Priority => _handler.Priority;
        public EventChannel Channel => _handler.Channel;

        public EventHandlerWrapper(IEventHandler<TEvent> handler)
        {
            _handler = handler;
        }

        public void Invoke(IEvent evt)
        {
            _handler.Handle((TEvent)evt);
        }
        
        public bool EqualsHandler(IEventHandler<TEvent> other)
        {
            return ReferenceEquals(_handler, other);
        }
    }
}