using System.Collections.Generic;
    using Audio.Core.Data;
namespace Audio.Runtime.Systems
{
    

    internal sealed class AudioMusicDirector
    {
        private AudioId? _current;
        private readonly Dictionary<string, AudioId> _states = new();

        public void RegisterState(string state, AudioId musicId)
        {
            _states[state] = musicId;
        }

        public AudioId? GetMusic(string state)
        {
            return _states.TryGetValue(state, out var id) ? id : null;
        }

        public bool ShouldSwitch(string state, out AudioId newMusic)
        {
            newMusic = default;

            if (!_states.TryGetValue(state, out var id))
                return false;

            if (_current.HasValue && _current.Value.Equals(id))
                return false;

            _current = id;
            newMusic = id;
            return true;
        }
    }
}