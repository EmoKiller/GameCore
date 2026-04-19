namespace Game.Application.Observability.Events
{
    public enum TraceEventType
    {
        EventPublished,
        HandlerStarted,
        HandlerCompleted,
        HandlerFailed,
        MiddlewareBefore,
        MiddlewareAfter
    }
}