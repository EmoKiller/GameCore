using System.Threading;
using System.Threading.Tasks;
using Audio.Core.Data;
using Audio.Infrastructure.AddressableAudioProvider;
using Audio.Runtime.Handles;
using Audio.Runtime.Systems;
using Game.Application.Core;
using UnityEngine;
using UnityEngine.Audio;
namespace Audio.Runtime
{
    public interface IAudioService : IService
    {
        IAudioHandle Play(AudioId id);
        IAudioHandle PlayOneShot(AudioId id);
        
        void Stop(AudioId id);
        void StopAll();

        void SetVolume(EAudioChannelType channel, float volume);
    }
    public sealed class AudioService : IAudioService
    {
        private readonly AudioRegistry _registry;
        private readonly AudioPlayer _player;
        private readonly AudioSettingsService _settingsService;
        private readonly AudioMixerController _mixer;

        public AudioService(IAudioProvider provider, AudioMixer mixer)
        {
            _settingsService = new AudioSettingsService();
            _mixer = new AudioMixerController(mixer);

            _registry = new AudioRegistry(provider);
            _player = new AudioPlayer(_registry, _mixer);
        }

        public async Task InitializeAsync()
        {
            await _registry.InitializeAsync();

            var settings = _settingsService.Load();
            ApplySettings(settings);
        }

        public void ApplySettings(Core.Data.AudioSettings settings)
        {
            _mixer.SetMaster(settings.Master);
            _mixer.SetVolume(EAudioChannelType.Music, settings.Music);
            _mixer.SetVolume(EAudioChannelType.SFX, settings.SFX);
            _mixer.SetVolume(EAudioChannelType.UI, settings.UI);
            _mixer.SetVolume(EAudioChannelType.Voice, settings.Voice);
        }

        public IAudioHandle Play(AudioId id)
            => _player.Play(id);

        public IAudioHandle PlayOneShot(AudioId id)
            => _player.Play(id);

        public async System.Threading.Tasks.Task<IAudioHandle> PlayAsync(AudioId id, System.Threading.CancellationToken ct)
            => await _player.PlayAsync(id, ct);

        public void Stop(AudioId id)
            => _player.Stop(id);

        public void StopAll()
            => _player.StopAll();

        public void SetVolume(EAudioChannelType channel, float volume)
            => _mixer.SetVolume(channel, volume);
    }
}
//container.RegisterSingleton<Audio.Core.Abstractions.IAudioProvider, Audio.Infrastructure.Addressables.AddressableAudioProvider>();
//container.RegisterSingleton<Audio.Core.Abstractions.IAudioService, Audio.Runtime.Services.AudioService>();