using UnityEngine;
using TMPro;
using System.Collections;

namespace MagicVillageDash.World
{
    [DisallowMultipleComponent]
    public sealed class WorldReward3D : MonoBehaviour, IWorldReward3D
    {
        [Header("Refs")]
        [SerializeField] private TextMeshPro text;
        [SerializeField] private Animator animator;

        private void Awake()
        {
            if (!text) text = GetComponentInChildren<TextMeshPro>(true);
            if (!animator) animator = GetComponentInChildren<Animator>(true);
            text.gameObject.SetActive(false);
        }

        public void Play(int coins, Vector3 position)
        {
            text.gameObject.SetActive(true);
            transform.position = position;

            if (text) text.text = coins >= 0 ? $"+{coins} coins" : coins.ToString();
            StartCoroutine(WaitForDisabled(0.5f));
        }
        IEnumerator WaitForDisabled(float time)
        {
            yield return new WaitForSeconds(time);
            text.gameObject.SetActive(false);
        }
    }
}
