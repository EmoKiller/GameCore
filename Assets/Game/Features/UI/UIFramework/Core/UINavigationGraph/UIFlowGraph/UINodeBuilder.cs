using UnityEngine;

public sealed class UINodeBuilder
{
    private readonly UINode _node;
    private readonly UIFlowGraph _graph;
    public UINodeBuilder(UINode node, UIFlowGraph graph)
    {
        _node = node;
        _graph = graph;
    }

    public UINodeBuilder AddEdge<TTarget>(
        string action,
        IUINavigationGuard guard = null,
        bool clearStack = false,
        bool allowBack = true)
    {
        var targetType = typeof(TTarget);

        _node.Edges.Add(new UIEdge
        {
            Action = action,
            TargetViewType = targetType,
            ClearStack = clearStack,
            AllowBack = allowBack,
            Guard = guard
        });

        return this;
    }

}

