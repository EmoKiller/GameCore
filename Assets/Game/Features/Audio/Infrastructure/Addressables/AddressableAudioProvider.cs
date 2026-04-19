using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
namespace Audio.Infrastructure.AddressableAudioProvider
{
    public interface IAudioProvider
    {
        Task<AudioClip> LoadAsync(string address, CancellationToken ct);
        void Release(AudioClip clip);
    }

    public sealed class AddressableAudioProvider : IAudioProvider
    {
        private readonly Dictionary<string, AudioClip> _cache = new();
        private readonly Dictionary<string, Task<AudioClip>> _inFlight = new();

        public async Task<AudioClip> LoadAsync(string address, CancellationToken ct)
        {
            if (_cache.TryGetValue(address, out var cached))
                return cached;

            if (_inFlight.TryGetValue(address, out var existing))
                return await existing;

            var task = LoadInternal(address, ct);
            _inFlight[address] = task;

            var clip = await task;

            _inFlight.Remove(address);
            _cache[address] = clip;

            return clip;
        }

        private async Task<AudioClip> LoadInternal(string address, CancellationToken ct)
        {
            var handle = Addressables.LoadAssetAsync<AudioClip>(address);

            while (!handle.IsDone)
            {
                ct.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            return handle.Result;
        }

        public void Release(AudioClip clip)
        {
            if (clip == null) return;

            foreach (var kv in _cache)
            {
                if (kv.Value == clip)
                {
                    Addressables.Release(clip);
                    _cache.Remove(kv.Key);
                    break;
                }
            }
        }
    }
}