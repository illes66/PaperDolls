using System;
using System.Collections.Generic;
using UnityEngine;

namespace PaperDollsGame.Content
{
    public enum ItemSlot
    {
        Dress,
        Top,
        Bottom,
        Shoes,
        Hair,
        Accessory
    }

    [CreateAssetMenu(menuName = "Paper Dolls/Item Definition")]
    public sealed class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private ItemSlot slot;
        [SerializeField] private List<string> tags = new List<string>();
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject visualPrefab;

        internal void Initialize(string id, string displayName, ItemSlot slot, List<string> tags, Sprite icon, GameObject visualPrefab)
        {
            this.id = id;
            this.displayName = displayName;
            this.slot = slot;
            this.tags = tags ?? new List<string>();
            this.icon = icon;
            this.visualPrefab = visualPrefab;
        }

        public string Id { get { return id; } }
        public string DisplayName { get { return displayName; } }
        public ItemSlot Slot { get { return slot; } }
        public IReadOnlyList<string> Tags { get { return tags; } }
        public Sprite Icon { get { return icon; } }
        public GameObject VisualPrefab { get { return visualPrefab; } }
    }
}
