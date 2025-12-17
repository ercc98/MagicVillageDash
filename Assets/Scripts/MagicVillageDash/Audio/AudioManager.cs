using UnityEngine;
using ErccDev.Foundation.Audio;
namespace MagicVillageDash.Audio
{
    public sealed class AudioManager : AudioManagerBase
    {
        public static AudioManager Instance { get; private set; }

        protected override void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            base.Awake();
        }

        // Typed convenience methods
        public void Play(AmbientId id) => Play(id, SoundCategory.Ambient);
        public void Play(MusicId   id) => Play(id, SoundCategory.Music);
        public void Play(SfxId     id) => Play(id, SoundCategory.SFX);
        public void Play(UIId      id) => Play(id, SoundCategory.UI);
        public void Play(VoiceId   id) => Play(id, SoundCategory.Voice);

        public void PlayLoop(AmbientId id) => PlayLoop(id, SoundCategory.Ambient);
        public void PlayLoop(MusicId   id) => PlayLoop(id, SoundCategory.Music);
        public void PlayLoop(SfxId     id) => PlayLoop(id, SoundCategory.SFX);

        public override void StopLoop(SoundCategory cat)=> base.StopLoop(cat);
        public void StopLoop(MusicId id)                => base.StopLoop(id, SoundCategory.Music);
        public void StopLoop(AmbientId id)              => base.StopLoop(id, SoundCategory.Ambient);
        public void StopLoop(SfxId id)                  => base.StopLoop(id, SoundCategory.SFX);

        public void SetMusicVolume(float v01)  => SetVolume(SoundCategory.Music, v01);
        public void SetSfxVolume(float v01)    => SetVolume(SoundCategory.SFX, v01);
        public void ToggleMusic(bool on)       => Toggle(SoundCategory.Music, on);
        public void ToggleSfx(bool on)         => Toggle(SoundCategory.SFX, on);
    }
}