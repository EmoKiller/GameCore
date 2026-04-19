using System.Collections.Generic;
using Game.Application.Debugging.Core;

namespace Game.Application.Debugging.Runtime
{
    public sealed class EventDependencyGraphBuilder
    {
        public readonly List<EventTraceNode> Nodes = new();
        public readonly List<TraceEdge> Edges = new();

        public void Add(EventTraceNode node)
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