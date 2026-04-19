
using Game.Application.Observability.Events;

public sealed class EventTraceEntry
{
    public string EventName;
    public TraceEventType Type;

    public long Timestamp;
    public float DurationMs;

    public string Channel;
    public int Priority;

    public string SourceSystem;
}