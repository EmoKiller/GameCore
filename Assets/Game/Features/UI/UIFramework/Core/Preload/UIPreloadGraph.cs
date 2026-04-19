using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class UIPreloadGraph
{
    private readonly Dictionary<Type, IUIPreloadNode> _map;

    public UIPreloadGraph(IEnumerable<IUIPreloadNode> nodes)
    {
        _map = nodes.ToDictionary(n => n.ViewType);
    }

    public IUIPreloadNode Get(Type type)
    {
        return _map.TryGetValue(type, out var node) ? node : null;
    }

    public IEnumerable<IUIPreloadNode> GetAll() => _map.Values;
}