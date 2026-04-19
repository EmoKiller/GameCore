using System.Collections.Generic;

public interface IEventProfiler
{
    void Record(EventTraceEntry entry);
    IReadOnlyList<EventProfileStats> GetStats();
}