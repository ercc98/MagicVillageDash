using UnityEngine;

namespace MagicVillageDash.Character.CharacterAnimator
{
    public interface IMovementAnimator
    {
        void TurnLeft();
        void TurnRight();
        void MovingSpeed(float speed);
        void Crouch(bool isCrouching);
        void Jump();
        void Landing();
    }
}
