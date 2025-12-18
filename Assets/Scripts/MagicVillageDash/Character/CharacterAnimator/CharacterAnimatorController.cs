using UnityEngine;

namespace MagicVillageDash.Character.CharacterAnimator
{
    public class CharacterAnimatorController : MonoBehaviour, IMovementAnimator, IDeathAnimator
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
            if (speed > 0) animatorController.SetBool("Walk", true);
            else animatorController.SetBool("Walk", false);
            
            animatorController.SetFloat("Speed", speed);
        }

        public void TurnLeft()
        {
            animatorController.SetTrigger("RightSpin");
        }

        public void TurnRight()
        {
            animatorController.SetTrigger("LeftSpin");
        }

        public void Die()
        {
            animatorController.SetBool("Dead", true);
        }

        public void Revive()
        {
            animatorController.SetBool("Dead", false);
        }

        public void SetDead(bool isDead)
        {
            animatorController.SetBool("Dead", isDead);
        }

        public void Idle()
        {
            animatorController.SetBool("Walk", false);
        }
    }
}
