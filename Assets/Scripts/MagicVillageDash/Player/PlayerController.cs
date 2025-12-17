using ErccDev.Foundation.Core.Gameplay;
using MagicVillageDash.Character;
using UnityEngine;

namespace MagicVillageDash.Player
{
    [DisallowMultipleComponent]
    public class PlayerController : CharacterControllerBase
    {
        protected override void OnHazardHitInternal(Vector3 hazardHitPosition)
        {
            GameEvents.RaiseGameOver();
        }
    }
}
