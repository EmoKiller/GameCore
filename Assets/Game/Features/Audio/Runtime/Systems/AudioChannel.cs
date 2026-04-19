using Audio.Core.Data;
using Audio.Runtime.Handles;
namespace Audio.Runtime.Systems
{
    

    internal sealed class AudioChannel
    {
        private readonly AudioPool _pool;
        private float _volume = 1f;

        public AudioChannel()
        {
            _pool = new AudioPool();
        }

        public AudioHandle Play(AudioData data)
        {
            var source = _pool.Get();

            source.clip = data.Clip;
            source.loop = data.Loop;
            source.volume = data.Volume * _volume;
            source.Play();

            return new AudioHandle(source);
        }

        public AudioHandle PlayOneShot(AudioData data)
        {
            var source = _pool.Get();
            source.PlayOneShot(data.Clip, data.Volume * _volume);
            return new AudioHandle(source);
        }

        public void StopAll()
        {
            _pool.StopAll();
        }

        public void SetVolume(float volume)
        {
            _volume = volume;
        }
    }
}