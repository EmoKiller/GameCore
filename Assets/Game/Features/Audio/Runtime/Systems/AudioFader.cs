using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace Audio.Runtime.Systems
{
    
    internal static class AudioFader
    {
        public static async Task FadeIn(AudioSource source, float duration, float targetVolume, CancellationToken ct)
        {
            float time = 0f;
            source.volume = 0f;

            while (time < duration)
            {
                if (ct.IsCancellationRequested) return;

                time += Time.deltaTime;
                source.volume = Mathf.Lerp(0f, targetVolume, time / duration);
                await Task.Yield();
            }

            source.volume = targetVolume;
        }

        public static async Task FadeOut(AudioSource source, float duration, CancellationToken ct)
        {
            float startVolume = source.volume;
            float time = 0f;

            while (time < duration)
            {
                if (ct.IsCancellationRequested) return;

                time += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, 0f, time / duration);
                await Task.Yield();
            }

            source.volume = 0f;
            source.Stop();
        }
    }
}