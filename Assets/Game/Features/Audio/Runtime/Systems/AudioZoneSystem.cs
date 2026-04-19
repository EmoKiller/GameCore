using UnityEngine;
    using UnityEngine.Audio;
namespace Audio.Runtime.Systems
{
    

    internal sealed class AudioZoneSystem
    {
        private AudioMixerSnapshot _current;

        public void EnterZone(AudioMixerSnapshot snapshot, float transitionTime)
        {
            if (snapshot == null) return;

            _current = snapshot;
            snapshot.TransitionTo(transitionTime);
        }
    }
}