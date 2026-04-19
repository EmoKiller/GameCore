using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using Game.Application.Events.Debugging;

namespace Game.Application.Events.Middleware
{
    public sealed class LoggingMiddleware : IEventMiddleware
    {
        public UniTask InvokeAsync(EventContext context, EventExecutionDelegate next)
        {
            UnityEngine.Debug.Log($"[Event] {context.Event.GetType().Name} | Channel: {context.Channel}");

            return next(context);
        }
    }
    public sealed class ValidationMiddleware : IEventMiddleware
    {
        private readonly List<IEventValidator> _validators;

        public ValidationMiddleware(List<IEventValidator> validators)
        {
            _validators = validators;
        }

        public UniTask InvokeAsync(EventContext context, EventExecutionDelegate next)
        {
            for (int i = 0; i < _validators.Count; i++)
            {
                if (!_validators[i].Validate(context.Event))
                {
                    context.Cancel();
                    return UniTask.CompletedTask;
                }
            }

            return next(context);
        }
    }
    public sealed class RetryMiddleware : IEventMiddleware
    {
        private readonly int _maxRetry;

        public RetryMiddleware(int maxRetry)
        {
            _maxRetry = maxRetry;
        }

        public async UniTask InvokeAsync(EventContext context, EventExecutionDelegate next)
        {
            int attempt = 0;

            while (true)
            {
                try
                {
                    await next(context);
                    return;
                }
                catch (Exception)
                {
                    attempt++;

                    if (attempt > _maxRetry)
                        throw;
                }
            }
        }
    }
    public sealed class ProfilingMiddleware : IEventMiddleware
    {
        public async UniTask InvokeAsync(EventContext context, EventExecutionDelegate next)
        {
            var sw = Stopwatch.StartNew();

            await next(context);

            sw.Stop();

            UnityEngine.Debug.Log(
                $"[Event Profiling] {context.Event.GetType().Name} took {sw.ElapsedMilliseconds} ms");
        }
    }
    public sealed class CancellationMiddleware : IEventMiddleware
    {
        public UniTask InvokeAsync(EventContext context, EventExecutionDelegate next)
        {
            if (context.Event is ICancellableEvent c && c.ShouldCancel())
            {
                context.Cancel();
                return UniTask.CompletedTask;
            }

            return next(context);
        }
    }

    /// <summary>
    /// Debug
    /// </summary>
    public sealed class EventTracingMiddleware : IEventMiddleware
    {
        private readonly EventTraceService _traceService;

        public EventTracingMiddleware(EventTraceService traceService)
        {
            _traceService = traceService;
        }

        public async UniTask InvokeAsync(EventContext context, EventExecutionDelegate next)
        {
            var trace = new EventTrace
            {
                EventName = context.Event.GetType().Name,
                Channel = context.Channel
            };

            var sw = Stopwatch.StartNew();

            await next(context);

            sw.Stop();

            trace.StartTime = 0;
            trace.EndTime = sw.ElapsedMilliseconds;

            _traceService.Add(trace);
        }
    }
}