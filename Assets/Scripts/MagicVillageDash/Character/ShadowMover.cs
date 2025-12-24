using UnityEngine;

namespace MagicVillageDash.Character
{
    public class ShadowMover : MonoBehaviour
    {
        [SerializeField] private Transform character;
        [SerializeField] private Transform shadow;
        [SerializeField] private Vector3 offset = Vector3.zero;

        void Update()
        {
            shadow.position = character.position - new Vector3(0, character.position.y, 0) + offset;
        }
    }
}
