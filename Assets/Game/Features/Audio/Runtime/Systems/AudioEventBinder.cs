using System;
using System.Collections.Generic;
using Audio.Core.Data;
namespace Audio.Runtime.Systems
{
    

    internal sealed class AudioEventBinder
    {
        private readonly Dictionary<string, AudioId> _map = new();

        public void Bind(string eventName, AudioId id)
        {
            _map[eventName] = id;
        }

        public bool TryGet(string eventName, out AudioId id)
        {
            return _map.TryGetValue(eventName, out id);
        }
    }
}