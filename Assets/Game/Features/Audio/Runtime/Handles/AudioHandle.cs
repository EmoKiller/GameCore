using System.Threading;
using System.Threading.Tasks;
using Audio.Runtime.Systems;
using UnityEngine;


namespace Audio.Runtime.Handles
{
    public interface IAudioHandle
    {
        void Stop();
        void Pause();
        void Resume();

        Task FadeIn(float duration, float targetVolume);
        Task FadeOut(float duration);

        bool IsPlaying { get; }
    }
    
    public sealed class AudioHandle : IAudioHandle
    {
        private readonly AudioSource _source;
        private CancellationTokenSource _cts;

        public AudioHandle(AudioSource source)
        {
            _source = source;
            _cts = new CancellationTokenSource();
        }

        public void Stop()
        {
            if (_source == null) return;
            _cts.Cancel();
            _source.Stop();
        }

        public void Pause()
        {
            if (_source == null) return;
            _source.Pause();
        }

        public void Resume()
        {
            if (_source == null) return;
            _source.UnPause();
        }

        public async Task FadeIn(float duration, float targetVolume)
        {
            await AudioFader.FadeIn(_source, duration, targetVolume, _cts.Token);
        }

        public async Task FadeOut(float duration)
        {
            await AudioFader.FadeOut(_source, duration, _cts.Token);
        }

        public bool IsPlaying => _source != null && _source.isPlaying;
    }
}