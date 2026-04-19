using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;

namespace Game.Application.Events.Debugging
{
    
    public sealed class EventTrace
    {
        public string EventName;
        public EventChannel Channel;

        public long StartTime;
        public long EndTime;

        public readonly List<HandlerTrace> Handlers = new();

        public long Duration => EndTime - StartTime;
    }

    public sealed class HandlerTrace
    {
        public string HandlerName;
        public long Duration;
    }
    public interface IEventTraceService
    {
        IReadOnlyList<EventTrace> GetTraces();
    }
    public sealed class EventTraceService : IEventTraceService
    {
        private const int Max = 100;

        private readonly EventTrace[] _buffer = new EventTrace[Max];
        private int _index;
        private int _count;

        public void Add(EventTrace trace)
        {
            _buffer[_index] = trace;
            _index = (_index + 1) % Max;

            if (_count < Max)
                _count++;
        }

        public IReadOnlyList<EventTrace> GetTraces()
        {
            var list = new List<EventTrace>(_count);

            for (int i = 0; i < _count; i++)
            {
                int idx = (_index - _count + i + Max) % Max;
                list.Add(_buffer[idx]);
            }

            return list;
        }
    }
    
}