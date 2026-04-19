
using UnityEngine;
namespace Audio.Runtime.Systems
{
    

    internal sealed class AudioSettingsService
    {
        private const string KEY = "AUDIO_SETTINGS";

        public Core.Data.AudioSettings Load()
        {
            if (!PlayerPrefs.HasKey(KEY))
                return new Core.Data.AudioSettings();

            return JsonUtility.FromJson<Core.Data.AudioSettings>(PlayerPrefs.GetString(KEY));
        }

        public void Save(Core.Data.AudioSettings settings)
        {
            var json = JsonUtility.ToJson(settings);
            PlayerPrefs.SetString(KEY, json);
            PlayerPrefs.Save();
        }
    }
}