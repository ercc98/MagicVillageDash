using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagicVillageDash.Data
{
    /// <summary>
    /// The only den-specific state the collection can't express: which owned structures are currently
    /// BUILT in the den, and WHERE. Ownership itself lives in the collection (CollectionProgressData) —
    /// discovering an entry (relic pickup or achievement unlock) makes it owned; this remembers the
    /// slot the player dropped it into so the den rebuilds identically next session. Tray = owned − placed.
    ///
    /// Mirrors AchievementData / CollectionProgressData and rides GameDataService's ScriptableObject
    /// list (playerdata.json). Pure data: the placement UI calls <c>GameDataService.SaveAll()</c> after
    /// mutating it.
    /// </summary>
    [CreateAssetMenu(menuName = "MagicVillageDash/Data/Den Placement Data", fileName = "DenPlacementData")]
    public sealed class DenPlacementData : ScriptableObject
    {
        /// <summary>One built structure: which collection entry sits in which scene slot.</summary>
        [Serializable]
        public struct Placement
        {
            public string slotId;
            public string entryId;
        }

        [Header("Built in the den")]
        [SerializeField] private List<Placement> placements = new();

        // Fast lookups, rebuilt lazily from the serialized list after a load.
        private Dictionary<string, string> _bySlot;   // slotId  -> entryId
        private Dictionary<string, string> _byEntry;  // entryId -> slotId

        public IReadOnlyList<Placement> Placements => placements;
        public int Count => placements.Count;

        public bool IsPlaced(string entryId)
        {
            if (string.IsNullOrEmpty(entryId)) return false;
            EnsureMaps();
            return _byEntry.ContainsKey(entryId);
        }

        public bool IsSlotOccupied(string slotId)
        {
            if (string.IsNullOrEmpty(slotId)) return false;
            EnsureMaps();
            return _bySlot.ContainsKey(slotId);
        }

        /// <summary>The entry built in a slot, or null if the slot is empty.</summary>
        public string EntryAt(string slotId)
        {
            if (string.IsNullOrEmpty(slotId)) return null;
            EnsureMaps();
            return _bySlot.TryGetValue(slotId, out var id) ? id : null;
        }

        /// <summary>The slot an entry is built in, or null if it's still in the tray.</summary>
        public string SlotOf(string entryId)
        {
            if (string.IsNullOrEmpty(entryId)) return null;
            EnsureMaps();
            return _byEntry.TryGetValue(entryId, out var slot) ? slot : null;
        }

        /// <summary>
        /// Builds <paramref name="entryId"/> into <paramref name="slotId"/>. Frees whatever was in the
        /// target slot and any prior placement of this entry (one entry lives in one slot at a time).
        /// </summary>
        public bool PlaceAt(string slotId, string entryId)
        {
            if (string.IsNullOrEmpty(slotId) || string.IsNullOrEmpty(entryId)) return false;

            placements.RemoveAll(p => p.slotId == slotId || p.entryId == entryId);
            placements.Add(new Placement { slotId = slotId, entryId = entryId });
            Invalidate();
            return true;
        }

        /// <summary>Returns a structure to the tray. Returns true if it was placed.</summary>
        public bool Unplace(string entryId)
        {
            if (string.IsNullOrEmpty(entryId)) return false;
            int removed = placements.RemoveAll(p => p.entryId == entryId);
            if (removed == 0) return false;
            Invalidate();
            return true;
        }

        /// <summary>Empties a slot. Returns true if something was built there.</summary>
        public bool ClearSlot(string slotId)
        {
            if (string.IsNullOrEmpty(slotId)) return false;
            int removed = placements.RemoveAll(p => p.slotId == slotId);
            if (removed == 0) return false;
            Invalidate();
            return true;
        }

        public void ResetAll()
        {
            placements.Clear();
            Invalidate();
        }

        /// <summary>Drops the cached lookups so they rebuild from the (freshly loaded) list.</summary>
        public void Invalidate()
        {
            _bySlot = null;
            _byEntry = null;
        }

        private void EnsureMaps()
        {
            if (_bySlot != null && _bySlot.Count == placements.Count) return;

            _bySlot = new Dictionary<string, string>(StringComparer.Ordinal);
            _byEntry = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var p in placements)
            {
                if (string.IsNullOrEmpty(p.slotId) || string.IsNullOrEmpty(p.entryId)) continue;
                _bySlot[p.slotId] = p.entryId;
                _byEntry[p.entryId] = p.slotId;
            }
        }
    }
}
