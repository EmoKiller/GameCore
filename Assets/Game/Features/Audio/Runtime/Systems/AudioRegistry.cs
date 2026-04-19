using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audio.Core.Data;
using Audio.Infrastructure.AddressableAudioProvider;
namespace Audio.Runtime.Systems
{
    

    internal sealed class AudioRegistry
    {
        private Dictionary<AudioId, AudioData> _map;
        private readonly IAudioProvider _provider;

        public AudioRegistry(IAudioProvider provider)
        {
            _provider = provider;
        }

        public Task InitializeAsync()
        {
            _map = new Dictionary<AudioId, AudioData>();
            return Task.CompletedTask;
        }

        public void Register(AudioData data)
        {
            _map[data.Id] = data;
        }

        // ✅ SINGLE SOURCE OF TRUTH
        public async Task<AudioData> GetAsync(AudioId id, CancellationToken ct = default)
        {
            if (!_map.TryGetValue(id, out var data))
                throw new KeyNotFoundException($"Audio not found: {id}");

            if (data.Clip == null && !string.IsNullOrEmpty(data.Address))
            {
                data.Clip = await _provider.LoadAsync(data.Address, ct);
            }

            return data;
        }
        public bool TryGetCached(AudioId id, out AudioData data)
        {
            return _map.TryGetValue(id, out data) && data.Clip != null;
        }
    }
}