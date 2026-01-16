using ErccDev.Foundation.Data;
using MagicVillageDash.Audio;
using UnityEngine;

namespace MagicVillageDash.Settings
{
    public class SettingsApplier : MonoBehaviour, ISettingsApplier
    {
        [Header("Optional")]
        [SerializeField] private MonoBehaviour audioManagerProvider; // should implement IMvdAudioManager (optional)

        private AudioManager audioManager;
        
        private void Awake()
        {
            audioManager = audioManagerProvider as AudioManager;
        }
        
        public void Apply(SettingsData settings)
        {
            if (settings == null) return;

            settings.Clamp();

            AudioListener.volume = settings.MasterVolume;

            QualitySettings.SetQualityLevel(settings.QualityLevel, applyExpensiveChanges: true);

            if (audioManager != null)
            {
                audioManager.SetMusicVolume(settings.MusicVolume);
                audioManager.SetSfxVolume(settings.SfxVolume);

                audioManager.SetMusicEnabled(settings.MusicEnabled);
                audioManager.SetAmbientEnabled(settings.AmbientEnabled);
                audioManager.SetSfxEnabled(settings.SfxEnabled);
                audioManager.SetUiEnabled(settings.UiEnabled);
                audioManager.SetVoiceEnabled(settings.VoiceEnabled);
            }

            // Vibration: apply via your own vibration service if you have one.
            // Example: VibrationService.Enabled = settings.Vibration;
        }
        
    }
}