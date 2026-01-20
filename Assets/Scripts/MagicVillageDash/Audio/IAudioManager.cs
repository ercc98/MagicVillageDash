using UnityEngine;

namespace MagicVillageDash
{
    public interface IAudioManager
    {
        void SetMusicVolume(float v);
        void SetSfxVolume(float v);
        void SetAmbientVolume(float v);
        void SetUiVolume(float v);
        void SetVoiceVolume(float v);

        void SetMusicEnabled(bool on);
        void SetAmbientEnabled(bool on);
        void SetSfxEnabled(bool on);
        void SetUiEnabled(bool on);
        void SetVoiceEnabled(bool on);
    }
}
