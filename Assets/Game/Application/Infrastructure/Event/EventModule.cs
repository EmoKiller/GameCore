
using System;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using Game.Application.Events;
using Game.Application.Events.Middleware;
using Game.Application.Events.Debugging;

public sealed class EventModule : BaseGameModule 
{
    public override string ModuleName => "EventModule";

    public override int InitializationOrder => 0;
    public override Type[] GetDependencies() => Type.EmptyTypes;

    private IEventBus _eventBus;

    protected override async UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct)
    {
        var bus = new EventBus();

        //bus.AddMiddleware(new LoggingMiddleware());
        //bus.AddMiddleware(new ProfilingMiddleware());
        // bus.AddMiddleware(new RetryMiddleware(2));

        // bus.AddMiddleware(new CancellationMiddleware());

        //debug
        // var traceService = new EventTraceService();
        // bus.AddMiddleware(new EventTracingMiddleware(traceService));

        _eventBus = bus;

        services.Register(_eventBus);

        await UniTask.CompletedTask;
    }
    public override void Shutdown()
    {
        
    }

    
}