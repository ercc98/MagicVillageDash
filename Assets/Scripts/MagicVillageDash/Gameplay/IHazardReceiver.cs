

using UnityEngine;

namespace MagicVillageDash.Gameplay
{
    public interface IHazardReceiver
    {
        void OnHazardHit(Vector3 hazardHitPosition);
    }
}
