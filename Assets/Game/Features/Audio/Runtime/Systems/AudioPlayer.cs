using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audio.Core.Data;
using Audio.Runtime.Handles;
using UnityEngine;
namespace Audio.Runtime.Systems
{
    

    internal sealed class AudioPlayer
    {
        private readonly AudioRegistry _registry;
        private readonly Dictionary<EAudioChannelType, AudioChannel> _channels;

        private readonly Dictionary<AudioId, List<AudioHandle>> _playing = new();

        private AudioHandle _currentMusic;
        private readonly AudioPolicy _policy;
        private readonly AudioLimiter _limiter;
        private readonly AudioMixerController _mixer;
        //private readonly AudioSpatialSystem _spatial;
        private readonly AudioEventBinder _binder;
        private readonly AudioMusicDirector _music;

        public AudioPlayer(AudioRegistry registry, AudioMixerController mixer)
        {
            _registry = registry;
            _mixer = mixer;

            _policy = new AudioPolicy();
            _limiter = new AudioLimiter();
           // _spatial = new AudioSpatialSystem();
            _binder = new AudioEventBinder();
            _music = new AudioMusicDirector();

            _channels = new Dictionary<EAudioChannelType, AudioChannel>
            {
                { EAudioChannelType.Music, new AudioChannel() },
                { EAudioChannelType.SFX, new AudioChannel() },
                { EAudioChannelType.UI, new AudioChannel() },
                { EAudioChannelType.Voice, new AudioChannel() }
            };
        }

        public async Task<AudioHandle> PlayAsync(AudioId id, CancellationToken ct)
        {
            var data = await _registry.GetAsync(id, ct);

            return PlayInternal(id, data);
        }

        public AudioHandle Play(AudioId id)
        {
            // ✅ SAFE PATH: chỉ dùng cache
            if (_registry.TryGetCached(id, out var cached))
            {
                return PlayInternal(id, cached);
            }

            Debug.LogWarning($"Audio not cached yet: {id}. Use PlayAsync instead.");
            return null;
        }
        private AudioHandle PlayInternal(AudioId id, AudioData data)
        {
            if (!_limiter.CanPlay(data.Channel))
                return null;

            _limiter.RegisterPlay(data.Channel);

            var handle = _channels[data.Channel].Play(data);

            Track(id, handle);

            // cleanup hook
            _ = WaitAndRelease(data.Channel, handle);

            return handle;
        }
        private async System.Threading.Tasks.Task WaitAndRelease(EAudioChannelType type, AudioHandle handle)
        {
            while (handle.IsPlaying)
                await System.Threading.Tasks.Task.Yield();

            _limiter.Unregister(type);
        }

        private AudioHandle PlayMusic(AudioData data)
        {
            var newHandle = _channels[EAudioChannelType.Music].Play(data);

            if (_currentMusic != null)
                _ = _currentMusic.FadeOut(1f);

            _ = newHandle.FadeIn(1f, data.Volume);

            _currentMusic = newHandle;

            return newHandle;
        }

        public void Stop(AudioId id)
        {
            if (_playing.TryGetValue(id, out var list))
            {
                foreach (var h in list)
                    h.Stop();

                _playing.Remove(id);
            }
        }

        public void StopAll()
        {
            foreach (var list in _playing.Values)
                foreach (var h in list)
                    h.Stop();

            _playing.Clear();

            foreach (var c in _channels.Values)
                c.StopAll();
        }

        public void SetVolume(EAudioChannelType type, float volume)
        {
            _channels[type].SetVolume(volume);
        }

        private void Track(AudioId id, AudioHandle handle)
        {
            if (!_playing.TryGetValue(id, out var list))
            {
                list = new List<AudioHandle>();
                _playing[id] = list;
            }

            list.Add(handle);
        }
    }
}