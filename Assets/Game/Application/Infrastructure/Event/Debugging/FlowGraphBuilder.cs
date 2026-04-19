using System.Collections.Generic;
using Game.Application.Debugging.Core;

namespace Game.Application.Debugging.Runtime
{
    public sealed class FlowGraphBuilder
    {
        public readonly List<FlowTraceNode> Nodes = new();
        public readonly List<TraceEdge> Edges = new();

        public void AddNode(FlowTraceNode node)
        {
            Nodes.Add(node);
        }

        public void Link(string fromId, string toId, string reason)
        {
            Edges.Add(new TraceEdge
            {
                FromId = fromId,
                ToId = toId,
                Reason = reason
            });
        }
    }
}