using System.Collections.Generic;
using UnityEngine;

namespace PaperDollsGame.Content
{
    public enum DollStyle
    {
        Casual,
        Formal,
        Playful,
        Classic,
        Modern
    }

    /// <summary>
    /// Base doll data. To create a new one: Assets > Create > Paper Dolls > Doll Definition,
    /// then set a stable, unique <c>id</c> (e.g. "doll_classic"); IDs are used by LiveOps JSON and must never change.
    /// </summary>
    [CreateAssetMenu(menuName = "Paper Dolls/Doll Definition")]
    public sealed class DollDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField, TextArea] private string description;
        [SerializeField] private GameObject modelPrefab;
        [SerializeField] private List<ItemDefinition> defaultOutfitItems = new List<ItemDefinition>();
        [SerializeField] private DollStyle style;

        public string Id { get { return id; } }
        public string DisplayName { get { return displayName; } }
        public string Description { get { return description; } }
        public GameObject ModelPrefab { get { return modelPrefab; } }
        public IReadOnlyList<ItemDefinition> DefaultOutfitItems { get { return defaultOutfitItems; } }
        public DollStyle Style { get { return style; } }
    }
}
