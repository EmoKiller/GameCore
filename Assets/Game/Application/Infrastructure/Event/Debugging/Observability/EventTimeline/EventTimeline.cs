using System.Collections.Generic;
using System.Linq;

public sealed class EventTimeline
{
    private readonly List<EventTraceEntry> _entries = new();

    public void Add(EventTraceEntry entry)
        => _entries.Add(entry);

    public IReadOnlyList<EventTraceEntry> GetSorted()
        => _entries.OrderBy(x => x.Timestamp).ToList();
}