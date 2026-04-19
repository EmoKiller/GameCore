using System.Collections.Generic;

namespace Game.Application.Debugging.Core
{
    public interface IDebugStream
    {
        void Push(DebugFrame frame);
        IReadOnlyList<DebugFrame> GetFrames();
        void Clear();
    }
    public sealed class DebugStream : IDebugStream
    {
        private readonly List<DebugFrame> _frames = new();

        public void Push(DebugFrame frame)
        {
            _frames.Add(frame);
        }

        public IReadOnlyList<DebugFrame> GetFrames()
            => _frames;

        public void Clear()
            => _frames.Clear();
    }
}