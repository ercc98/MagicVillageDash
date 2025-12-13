using System;
using UnityEngine;
using ErccDev.Foundation.Core.Gameplay;
using MagicVillageDash.Score;
using MagicVillageDash.Audio;
using MagicVillageDash.World;

namespace MagicVillageDash.Collectibles
{
    [RequireComponent(typeof(Collider))]
    public sealed class CoinCollectible : MonoBehaviour, ICollectible
    {
        [Header("Coin")]
        [SerializeField] private int value = 1;
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private string enemyTag = "Enemy";
        [SerializeField] private CollectedCoinFactory collectedCoinFactory;

        // cache rápido del contador para evitar Find en cada trigger
        [SerializeField] private static CoinCounter cachedCounter;
        public ChunkRoot Owner { get; internal set; }

        /// <summary>Raised when the coin is collected. Factory listens and recycles.</summary>
        public event Action<CoinCollectible, GameObject> Collected;

        public int Value => value;

        void Awake()
        {
            if (cachedCounter == null)
                cachedCounter = FindAnyObjectByType<CoinCounter>(FindObjectsInactive.Exclude);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag)) cachedCounter?.Add(value);

            TryCollect(other.gameObject);
            
            gameObject.SetActive(false);
        }

        public bool TryCollect(GameObject collector)
        {
            if (collector == null) return false;

            Collected?.Invoke(this, collector);
            
            Vector3 collectedCoinPosition = new(transform.position.x, transform.position.y, collectedCoinFactory.transform.position.z);
            collectedCoinFactory.Spawn(collectedCoinFactory.transform, collectedCoinPosition, transform.rotation);

            MagicVillageDashAudioManager.Instance?.Play(SfxId.Coin);

            return true;
        }

    }
}