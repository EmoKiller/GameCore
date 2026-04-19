using System;
using System.Collections.Generic;


public sealed class UIFlowGraph
{
    private readonly Dictionary<Type, UINode> _map = new();

    public UINodeBuilder AddNode<TView>()
    {
        var type = typeof(TView);

        if (_map.ContainsKey(type))
            throw new Exception($"Duplicate node: {type.Name}");

        var node = new UINode
        {
            ViewType = type
        };

        _map.Add(type, node);

        return new UINodeBuilder(node, this);
    }

    public UINode GetNode(Type viewType)
    {
        if (!_map.TryGetValue(viewType, out var node))
            throw new Exception($"Node not found: {viewType.Name}");

        return node;
    }

}