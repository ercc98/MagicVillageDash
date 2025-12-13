using UnityEngine;

namespace MagicVillageDash.Character.CharacterAnimator
{
    public class CharacterAnimatorController : MonoBehaviour, IMovementAnimator
    {
        [SerializeField] private Animator animatorController;
        [SerializeField] public Animator AnimatorController { private get => animatorController; set => animatorController = value; }
        
        
        public void Crouch(bool isCrouching)
        {
            animatorController.SetBool("IsCrouching", isCrouching);
        }

        public void Jump()
        {
            animatorController.SetTrigger("Jump");
        }

        public void Landing()
        {
            animatorController.SetTrigger("Land");
        }

        public void MovingSpeed(float speed)
        {
            animatorController.SetFloat("Speed", speed);
        }

        public void TurnLeft()
        {
            animatorController.SetTrigger("Spin");
        }

        public void TurnRight()
        {
            animatorController.SetTrigger("Spin");
        }

    }
}
