using ErccDev.Foundation.Core.Tutorial;
using TMPro;
using UnityEngine;

namespace MagicVillageDash.Tutorial
{
    public class TutorialOverlayUI : MonoBehaviour, ITutorialStepUI
    {
        [SerializeField] private GameObject arrowObject;
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private Animator arrowAnimator;

        void Awake()
        {
            Hide();
        }
        public void Show(TutorialStep step)
        {
            if (arrowObject == null) return;

            arrowObject.SetActive(true);

            if (instructionText != null) instructionText.text = step.instructionText;

            Play(arrowAnimator, step.arrowAnimationName);
        }

        public void Hide()
        {
            if (arrowAnimator != null)
                arrowAnimator.enabled = false;
            if (arrowObject != null)
                arrowObject.SetActive(false);
        }

        public void Play(Animator animator, string stateName)
        {
            if (!animator) return;
            if (string.IsNullOrWhiteSpace(stateName)) return;

            animator.enabled = true;

            // Ensures a clean restart even if the same state is played twice
            animator.Play(stateName, 0, 0f);

            // Forces immediate evaluation (important for UI animators)
            //animator.Update(0f);
        }
    }
}