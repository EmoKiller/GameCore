using System.Collections.Generic;
using Audio.Core.Data;
using UnityEngine;
namespace Audio.Runtime.Systems
{
    

    internal sealed class AudioLimiter
    {
        private readonly Dictionary<EAudioChannelType, int> _limits = new()
        {
            { EAudioChannelType.SFX, 10 },
            { EAudioChannelType.UI, 5 },
            { EAudioChannelType.Voice, 2 }
        };

        private readonly Dictionary<EAudioChannelType, int> _active = new();

        public bool CanPlay(EAudioChannelType type)
        {
            if (!_limits.ContainsKey(type))
                return true;

            _active.TryGetValue(type, out var current);
            return current < _limits[type];
        }

        public void RegisterPlay(EAudioChannelType type)
        {
            if (!_active.ContainsKey(type))
                _active[type] = 0;

            _active[type]++;
        }

        public void Unregister(EAudioChannelType type)
        {
            if (_active.ContainsKey(type))
                _active[type] = Mathf.Max(0, _active[type] - 1);
        }
    }
}