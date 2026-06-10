using System.Collections;
using UnityEngine;

namespace MagicVillageDash.Audio
{
    /// <summary>
    /// Kicks off looping music/ambient when a scene opens. Drop it on any scene that
    /// has no run lifecycle to drive audio (e.g. the den). AudioManager is a persistent
    /// singleton, so it may not exist yet if you enter the scene directly in the editor —
    /// we wait a few frames for it.
    /// </summary>
    public class SceneAudioStarter : MonoBehaviour
    {
        [Header("What to loop")]
        [SerializeField] private bool playMusic = true;
        [SerializeField] private MusicId music = MusicId.GameTheme4;

        [SerializeField] private bool playAmbient = true;
        [SerializeField] private AmbientId ambient = AmbientId.Ambient1;

        [Header("How long to wait for AudioManager")]
        [SerializeField, Min(0)] private int maxWaitFrames = 30;

        [Header("Stop the loops when leaving the scene")]
        [SerializeField] private bool stopOnSceneExit = true;

        private void OnEnable() => StartCoroutine(StartWhenReady());

        private void OnDisable()
        {
            if (!stopOnSceneExit) return;

            var audio = AudioManager.Instance;
            if (audio == null) return;

            if (playMusic)   audio.StopLoop(music);
            if (playAmbient) audio.StopLoop(ambient);
        }

        private IEnumerator StartWhenReady()
        {
            int frames = 0;
            while (AudioManager.Instance == null && frames++ < maxWaitFrames)
                yield return null;

            var audio = AudioManager.Instance;
            if (audio == null)
            {
                Debug.LogWarning($"{nameof(SceneAudioStarter)}: No AudioManager found; scene audio not started.", this);
                yield break;
            }

            if (playMusic)   audio.PlayLoop(music);
            if (playAmbient) audio.PlayLoop(ambient);
        }
    }
}
