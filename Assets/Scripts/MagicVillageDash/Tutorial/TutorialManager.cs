using ErccDev.Foundation.Core.Tutorial;
using UnityEngine;

namespace MagicVillageDash.Tutorial
{
    public class TutorialManager : TutorialManagerBase, ITutorialManager, ITutorialManagerConfig
    {

        protected override void Awake()
        {
            base.Awake();
            ui ??= FindAnyObjectByType<TutorialOverlayUI>(FindObjectsInactive.Exclude);
            
        }
        protected override void StartStep()
        {
            base.StartStep();
            Time.timeScale = 0f; // Pause game during tutorial
        }

        protected override void CompleteStep()
        {
            Time.timeScale = 1f; // Resume game after tutorial step
            base.CompleteStep();
        }

        
    }
}