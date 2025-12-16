using UnityEngine;

namespace MagicVillageDash.Character
{
    public interface IMovementController
    {
        void TurnLeft();
        void TurnRight();
        void MovingSpeed(float speed);
        void Crouch(bool isCrouching);
        void Jump();
    }
}
