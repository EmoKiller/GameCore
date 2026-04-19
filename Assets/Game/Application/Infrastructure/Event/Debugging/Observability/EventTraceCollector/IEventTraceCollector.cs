using System.Collections.Generic;

public interface IEventTraceCollector
{
    void Record(EventTraceEntry entry);
    IReadOnlyList<EventTraceEntry> GetTimeline();
    void Clear();
}