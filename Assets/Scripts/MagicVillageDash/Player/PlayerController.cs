using ErccDev.Foundation.Input.Swipe;
using MagicVillageDash.Input;
using MagicVillageDash.Runner;
using NUnit.Framework;
using UnityEngine;

namespace MagicVillageDash.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour selfLaneMoverProvider;
        [SerializeField] private MonoBehaviour selfRunnerInputProvider; 
        [SerializeField] private MonoBehaviour swipeInputProvider;
        private CharacterController selfCharacterController;
        private ILaneMover selfLaneMover;
        private IRunnerInputController inputController;
        private ISwipeInput swipeInput;

        void Awake()
        {
            inputController = selfRunnerInputProvider as IRunnerInputController;
            selfLaneMover = selfLaneMoverProvider as ILaneMover;
            swipeInput = swipeInputProvider as ISwipeInput;
            
            selfCharacterController = GetComponent<CharacterController>();
        }

        void OnEnable()
        {
            inputController.SetMover(selfLaneMover);

            inputController.SetInput(swipeInput);

            inputController.Activate();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
