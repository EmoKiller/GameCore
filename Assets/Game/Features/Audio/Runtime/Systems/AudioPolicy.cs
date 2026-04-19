namespace Audio.Runtime.Systems
{
    using Audio.Core.Data;

    internal sealed class AudioPolicy
    {
        public int GetPriority(EAudioChannelType type)
        {
            return type switch
            {
                EAudioChannelType.Music => 1,
                EAudioChannelType.Voice => 3,
                EAudioChannelType.SFX => 2,
                EAudioChannelType.UI => 4,
                _ => 0
            };
        }

        public bool CanInterrupt(EAudioChannelType newType, EAudioChannelType currentType)
        {
            return GetPriority(newType) >= GetPriority(currentType);
        }
    }
}