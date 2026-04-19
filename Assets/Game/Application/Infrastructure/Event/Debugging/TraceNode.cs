namespace Game.Application.Debugging.Core
{
    public abstract class TraceNode
    {
        public string Id;
        public string Name;

        public long Timestamp;

        public float DurationMs;
    }
    public sealed class FlowTraceNode : TraceNode
    {
        public string FlowName;
        public int StepIndex;

        public string ParentFlowId;
    }
    public sealed class EventTraceNode : TraceNode
    {
        public string EventType;

        public string ProducerId;
        public string ConsumerId;
    }
    public sealed class TraceEdge
    {
        public string FromId;
        public string ToId;

        public string Reason; // "triggered", "awaited", "subscribed"
    }
}