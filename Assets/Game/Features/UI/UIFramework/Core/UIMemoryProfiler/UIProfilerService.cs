using System;
using System.Collections.Generic;
using UnityEngine;

public interface IUIProfilerService
{
    void Record(UIProfilerEvent evt);

    IReadOnlyDictionary<Type, UIProfilerStats> GetStats();
}

public sealed class UIProfilerService : IUIProfilerService
{
    private readonly Dictionary<Type, UIProfilerStats> _stats = new();

    public void Record(UIProfilerEvent evt)
    {
        if (!_stats.TryGetValue(evt.ViewType, out var stat))
        {
            stat = new UIProfilerStats();
            _stats[evt.ViewType] = stat;
        }

        stat.Record(evt);
    }

    public IReadOnlyDictionary<Type, UIProfilerStats> GetStats()
        => _stats;

}
