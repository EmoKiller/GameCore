using System.Collections.Generic;

namespace Game.Application.Debugging.Core
{
    public enum DebugSourceType
    {
        EventBus,
        Flow,
        Loading,
        Reactive
    }

    public sealed class DebugFrame
    {
        public DebugSourceType Source;

        public string Name;
        public string Category;

        public long Timestamp;

        public float DurationMs;
        public float Progress;

        public Dictionary<string, object> Metadata;
    }
}