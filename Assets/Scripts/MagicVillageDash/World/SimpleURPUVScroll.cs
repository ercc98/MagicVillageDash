using UnityEngine;

namespace MagicVillageDash.World
{
    [RequireComponent(typeof(Renderer))]
    public class SimpleURPUVScroll : MonoBehaviour
    {
        [Header("URP Lit")]
        [SerializeField] private string textureProperty = "_BaseMap"; // Built-in: "_MainTex"

        [Header("Constant scroll (fallback)")]
        [SerializeField] private Vector2 uvPerSecond = new Vector2(0f, 1f); // V only by default

        [Header("Optional: link to world speed")]
        [SerializeField] private GameSpeedController speedSource; // if set, overrides uvPerSecond on V axis
        [SerializeField] private float metersPerUV = 1f;          // how many world meters equal 1.0 UV
        [SerializeField] private float directionMultiplier = 1f;  // set -1 to invert

        [Header("Material slot (for renderers with multiple materials)")]
        [SerializeField] private int materialIndex = 0;

        private Renderer _ren;
        private Material _mat;
        private Vector2  _offset;

        void Awake()
        {
            _ren = GetComponent<Renderer>();

            // Simple & safe for a prototype: create a per-instance material.
            // (Shipping tip: use MaterialPropertyBlock for better batching.)
            var mats = _ren.materials;
            materialIndex = Mathf.Clamp(materialIndex, 0, mats.Length - 1);
            _mat = mats[materialIndex];

            if (_mat.HasProperty(textureProperty))
                _offset = _mat.GetTextureOffset(textureProperty);
            else
                Debug.LogWarning($"[{nameof(SimpleURPUVScroll)}] Property '{textureProperty}' not found on material.", this);
        }

        void Update()
        {
            if (_mat == null || ! _mat.HasProperty(textureProperty)) return;

            if (speedSource != null)
            {
                // Scroll along V using world speed
                float uvDeltaV = (speedSource.CurrentSpeed * Time.deltaTime * directionMultiplier)
                                 / Mathf.Max(0.0001f, metersPerUV);
                _offset.y += uvDeltaV;
            }
            else
            {
                // Constant scroll on both axes (if you want)
                _offset += uvPerSecond * Time.deltaTime;
            }

            // Optional wrap to 0..1 to avoid large values over time
            _offset.x = Mathf.Repeat(_offset.x, 1f);
            _offset.y = Mathf.Repeat(_offset.y, 1f);

            _mat.SetTextureOffset(textureProperty, _offset);
        }
    }
}
