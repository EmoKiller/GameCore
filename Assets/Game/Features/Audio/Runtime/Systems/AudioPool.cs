using System.Collections.Generic;
using UnityEngine;
namespace Audio.Runtime.Systems
{
    

    internal sealed class AudioPool
    {
        private readonly Queue<AudioSource> _pool = new();

        public AudioSource Get()
        {
            if (_pool.Count > 0)
                return _pool.Dequeue();

            return Create();
        }

        private AudioSource Create()
        {
            var go = new GameObject("AudioSource");
            Object.DontDestroyOnLoad(go);
            return go.AddComponent<AudioSource>();
        }

        public void StopAll()
        {
            foreach (var source in _pool)
            {
                if (source != null)
                    source.Stop();
            }
        }
    }
}