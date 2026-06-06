using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagicVillageDash.Collections;

namespace MagicVillageDash.Den.Placement
{
    /// <summary>
    /// Bottom-of-screen tray: spawns one tappable cell per owned-but-unplaced structure into a
    /// horizontally swipeable, masked row. Cells are pooled from <see cref="cellPrefab"/> and bound
    /// automatically — nothing to wire per-cell in the inspector. Scrolling, masking and momentum are
    /// owned by a <see cref="ScrollRect"/>; this stays a pure view: the <see cref="DenPlacementController"/>
    /// hands it the current tray and a select callback, and tapping a cell forwards its entry straight back.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DenTrayCarouselUI : MonoBehaviour
    {
        [Header("Cell spawning")]
        [Tooltip("Button prefab carrying a DenTrayItemView. One instance is spawned per tray item, " +
                 "parented under the scroll's content and bound automatically.")]
        [SerializeField] private DenTrayItemView cellPrefab;
        [Tooltip("The scroll view that masks the row and handles swipe. Cells spawn under its 'content'.")]
        [SerializeField] private ScrollRect scroll;

        private readonly List<DenTrayItemView> _cells = new();
        private IReadOnlyList<ModelCollectionEntry> _items;
        private Action<ModelCollectionEntry> _onSelect;

        private int ItemCount => _items?.Count ?? 0;
        private RectTransform Content => scroll != null ? scroll.content : null;

        /// <summary>Shows a fresh tray. Call again after any placement to refresh.</summary>
        public void Show(IReadOnlyList<ModelCollectionEntry> items, Action<ModelCollectionEntry> onSelect)
        {
            _items = items;
            _onSelect = onSelect;
            EnsureCells(ItemCount);
            Render();
        }

        /// <summary>Grows the pool so there's one cell per item, each parented under the scroll's content.</summary>
        private void EnsureCells(int needed)
        {
            if (cellPrefab == null || Content == null) return;
            while (_cells.Count < needed)
            {
                var cell = Instantiate(cellPrefab, Content);
                cell.name = $"TrayCell_{_cells.Count}";
                _cells.Add(cell);
            }
        }

        private void Render()
        {
            for (int i = 0; i < _cells.Count; i++)
            {
                if (_cells[i] == null) continue;
                bool used = i < ItemCount;
                _cells[i].gameObject.SetActive(used);
                if (used) _cells[i].Bind(_items[i], _onSelect);
            }

            // Snap back to the start so a refreshed tray always opens on the first cell.
            if (scroll != null) scroll.horizontalNormalizedPosition = 0f;
        }
    }
}
