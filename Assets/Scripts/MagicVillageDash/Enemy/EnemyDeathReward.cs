using UnityEngine;
using MagicVillageDash.Score;
using MagicVillageDash.World;

namespace MagicVillageDash.Enemy
{
    [DisallowMultipleComponent]
    public sealed class EnemyDeathReward : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CoinCounter coinCounterProvider;
        [SerializeField] private WorldReward3D worldReward3DProvider;
        [SerializeField] private EnemyController enemy;

        ICoinCounter coinCounter;
        IWorldReward3D worldReward3D;

        private void Awake()
        {
            if (!enemy) enemy = GetComponent<EnemyController>();
            coinCounter = coinCounterProvider as ICoinCounter;
            worldReward3D = worldReward3DProvider as IWorldReward3D;
            if (coinCounter == null) Debug.LogError("EnemyRewardListener: coinCounterProvider must implement ICoinCounter.", this);
            if (worldReward3D == null) Debug.LogError("EnemyRewardListener: worldReward3DProvider must implement IWorldReward3D.", this);
        }

        private void OnEnable()
        {
            if (enemy != null) enemy.OnReward += HandleReward;
        }

        private void OnDisable()
        {
            if (enemy != null) enemy.OnReward -= HandleReward;
        }

        private void HandleReward(int coins, Vector3 offset)
        {
            coinCounter?.Add(coins);
            worldReward3D?.Play(coins, offset);
        }
    }
}
