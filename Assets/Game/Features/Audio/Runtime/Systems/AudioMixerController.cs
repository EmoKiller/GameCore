namespace Audio.Runtime.Systems
{
    using Audio.Core.Data;
    using UnityEngine;
    using UnityEngine.Audio;

    internal sealed class AudioMixerController
    {
        private readonly AudioMixer _mixer;

        public AudioMixerController(AudioMixer mixer)
        {
            _mixer = mixer;
        }

        public void SetVolume(EAudioChannelType type, float volume)
        {
            float db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
            _mixer.SetFloat(type.ToString(), db);
        }

        public void SetMaster(float volume)
        {
            float db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
            _mixer.SetFloat("Master", db);
        }
    }
}