using UnityEngine;
using UnityEngine.UI;
using ErccDev.Foundation.Audio;
using ErccDev.Foundation.Loader;
using ErccDev.Foundation.Core.Collection;
using MagicVillageDash.Audio;
using MagicVillageDash.Collections;
using MagicVillageDash.Data;
using MagicVillageDash.Den.Placement;

namespace MagicVillageDash.UI
{
    /// <summary>
    /// The doorway from a finished run into the den. Lives on the game-over panel: when that panel
    /// activates, <see cref="OnEnable"/> badges the button if the run left any owned-but-unplaced
    /// structures waiting (tray = owned − placed), and tapping it loads the den. Read-only against the
    /// collection — discoveries happen during the run; this only reflects and routes.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DenEntryButton : MonoBehaviour
    {
        [Header("Ownership sources (same SOs the den reads)")]
        [SerializeField] private CollectionCatalog catalog;
        [SerializeField] private CollectionProgressData collectionProgress;
        [SerializeField] private DenPlacementData placement;

        [Header("UI")]
        [SerializeField] private Button button;
        [Tooltip("Shown only when there's at least one unplaced structure to go build.")]
        [SerializeField] private GameObject newDropBadge;
        [Tooltip("Optional: hide the whole button until the den has earned its first structure.")]
        [SerializeField] private bool hideWhenNothingToPlace = false;

        [Header("Scene")]
        [SerializeField] private string denSceneName = "DenScene";

        void Awake()
        {
            if (button == null) button = GetComponent<Button>();
            if (button != null) button.onClick.AddListener(OnClick);
        }

        void OnEnable() => Refresh();

        /// <summary>Re-reads ownership and shows/hides the badge. Cheap; safe to call whenever.</summary>
        public void Refresh()
        {
            // The run mutated these SOs in memory (and the collection saves on discovery), so the live
            // values are already current — just drop the cached lookups before counting.
            collectionProgress?.Invalidate();
            placement?.Invalidate();

            int unplaced = DenInventory.CountUnplaced(catalog, collectionProgress, placement);
            bool hasWork = unplaced > 0;

            if (newDropBadge != null) newDropBadge.SetActive(hasWork);
            if (hideWhenNothingToPlace && button != null) button.gameObject.SetActive(hasWork);
        }

        private void OnClick()
        {
            AudioManager.Instance?.Play(UIId.Accept);
            AudioManager.Instance?.StopLoop(SoundCategory.Music);
            Time.timeScale = 1f;                 // game-over froze it; clear before leaving the scene
            SceneLoader.Instance.LoadSceneAsync(denSceneName);
        }
    }
}
