using System;
using UnityEngine;

namespace MagicVillageDash.Collectibles
{
    public sealed class CollectedCoinCollectible : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Animator animator;
        [SerializeField] private CollectedCoinFactory collectedCoinFactory;

        void Awake()
        {
            if (animator == null) animator = GetComponent<Animator>();
            animator.SetTrigger("Collected");
        }

        public void Recycle()
        {
            collectedCoinFactory.Recycle(this);
        }
    }
}