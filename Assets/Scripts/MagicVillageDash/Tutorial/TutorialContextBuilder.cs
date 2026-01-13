using UnityEngine;
using ErccDev.Foundation.Core.Tutorial;
using ErccDev.Foundation.Input.Swipe;
using MagicVillageDash.Character;
using MagicVillageDash.Player;

namespace MagicVillageDash.Tutorial
{
    public sealed class TutorialContextBuilder : MonoBehaviour, ITutorialContextBuilder
    {
        [Header("Providers (assign in Inspector)")]
        [SerializeField] private MonoBehaviour swipeInputProvider;          
        [SerializeField] private MonoBehaviour movementControllerProvider;  

        [Header("Optional Auto-Find (fallback)")]
        [SerializeField] private bool autoFindIfMissing = true;

        public ITutorialContext Build()
        {
            var swipe = swipeInputProvider as ISwipeInput;
            var move  = movementControllerProvider as IMovementController;

            if (autoFindIfMissing)
            {
                if (swipe == null)
                {
                    var swipeMb = FindAnyObjectByType<SwipeInputSystem>(FindObjectsInactive.Exclude);
                    swipe = swipeMb as ISwipeInput;
                }

                if (move == null)
                {
                    var moveMb = FindAnyObjectByType<PlayerController>(FindObjectsInactive.Exclude);
                    move = moveMb as IMovementController;
                }
            }

#if UNITY_EDITOR
            if (swipe == null)
                Debug.LogWarning("[TutorialContextBuilder] Missing ISwipeInput (assign swipeInputProvider).", this);

            if (move == null)
                Debug.LogWarning("[TutorialContextBuilder] Missing IMovementController (assign movementControllerProvider).", this);
#endif

            return new TutorialContext()
                .Add<ISwipeInput>(swipe)
                .Add<IMovementController>(move);
        }
    }
}