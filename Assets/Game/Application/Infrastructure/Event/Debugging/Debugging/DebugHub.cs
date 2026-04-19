namespace Game.Application.Debugging.Core
{
    public interface IDebugHub
    {
        IDebugStream EventStream { get; }
        IDebugStream FlowStream { get; }
        IDebugStream LoadingStream { get; }
    }
    public sealed class DebugHub : IDebugHub
    {
        public IDebugStream EventStream { get; } = new DebugStream();
        public IDebugStream FlowStream { get; } = new DebugStream();
        public IDebugStream LoadingStream { get; } = new DebugStream();
    }
}