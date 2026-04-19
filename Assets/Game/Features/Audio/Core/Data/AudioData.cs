using System;
using UnityEngine;
namespace Audio.Core.Data
{
    

    [Serializable]
    public class AudioData
    {
        public AudioId Id;

        // 🔥 Addressables key (Phase 3)
        public string Address;

        public AudioClip Clip;

        public EAudioChannelType Channel;
        public bool Loop;
        public float Volume = 1f;
    }
}