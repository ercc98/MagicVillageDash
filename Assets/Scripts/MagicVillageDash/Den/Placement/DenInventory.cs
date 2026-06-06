using System.Collections.Generic;
using ErccDev.Foundation.Core.Collection;
using MagicVillageDash.Collections;
using MagicVillageDash.Data;

namespace MagicVillageDash.Den.Placement
{
    /// <summary>
    /// One source of truth for "tray = owned − placed". The den controller builds its carousel from
    /// it; the runner's game-over den button reads its count to decide whether to badge. Pure query over
    /// the catalog + collection ownership + placement state — no scene, no UI, no side effects.
    /// </summary>
    public static class DenInventory
    {
        /// <summary>Every owned-but-not-yet-built structure, in catalog order.</summary>
        public static IEnumerable<ModelCollectionEntry> EnumerateUnplaced(
            CollectionCatalog catalog, CollectionProgressData progress, DenPlacementData placement)
        {
            if (catalog == null) yield break;

            foreach (var def in catalog.Entries)
            {
                if (def is not ModelCollectionEntry entry) continue;                       // only placeable structures
                if (progress != null && !progress.IsDiscovered(entry.entryId)) continue;   // not owned yet
                if (placement != null && placement.IsPlaced(entry.entryId)) continue;      // already built
                yield return entry;
            }
        }

        /// <summary>How many owned structures are still sitting in the tray.</summary>
        public static int CountUnplaced(
            CollectionCatalog catalog, CollectionProgressData progress, DenPlacementData placement)
        {
            int n = 0;
            foreach (var _ in EnumerateUnplaced(catalog, progress, placement)) n++;
            return n;
        }
    }
}
