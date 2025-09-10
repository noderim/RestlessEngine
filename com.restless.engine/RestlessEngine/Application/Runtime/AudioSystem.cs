using RestlessEngine.Application.Settings;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace RestlessEngine.Application.Runtime
{
    public class AudioSystem : SingletonSystem<AudioSystem>, ISettingsSystem
    {
        [SerializeField]
        public Settings.AudioSettings audioSettings { get; private set; }
        public UnityEvent onSettingsApply { get; set; }

        [Space(10)]
        public AudioMixer audioMixer;
        [Space(10)]
        public AudioMixerGroup masterMixerGroup;
        public AudioMixerGroup musicMixerGroup;
        public AudioMixerGroup sfxMixerGroup;
        public AudioMixerGroup uiMixerGroup;
        public AudioMixerGroup ambientMixerGroup;

        public void SetVolume(AudioMixerGroup MixerGroup, float volume)
        {
            // Convert linear volume (0.0 to 1.0) to decibels (-80dB to 0dB)
            float dB;
            if (volume <= 0.01f) // Threshold for muting
            {
                dB = -80f; // Mute
            }
            else
            {
                dB = Mathf.Log10(Mathf.Clamp(volume, 0.01f, 1.0f)) * 20;
            }
            string VolumeParameter = MixerGroup.name + "Volume";
            audioMixer.SetFloat(VolumeParameter, dB);
        }

        public void ApplySettings()
        {
            SetVolume(masterMixerGroup, audioSettings.MasterVolume);
            SetVolume(musicMixerGroup, audioSettings.MusicVolume);
            SetVolume(sfxMixerGroup, audioSettings.SFXVolume);
            SetVolume(uiMixerGroup, audioSettings.UIVolume);
            SetVolume(ambientMixerGroup, audioSettings.AmbientVolume);

            onSettingsApply?.Invoke();
        }

        public void GetSettings()
        {
            audioSettings = SettingsManager.Instance.GetCurrentSettings().audioSettings;
        }

        public void LoadSettings(SettingsData settings)
        {
            audioSettings = settings.audioSettings;
        }
    }
}
