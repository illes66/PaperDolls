using System.Collections.Generic;
using UnityEngine;

namespace PaperDollsGame.Content
{
    /// <summary>
    /// Album (catalog book) data. To create a new one: Assets > Create > Paper Dolls > Album Definition,
    /// set a stable, unique <c>id</c> (e.g. "album_001"), assign its doll, cover, items and book prefab,
    /// then add it to the albums list of the ContentCatalog. IDs are used by LiveOps JSON and must never change.
    /// </summary>
    [CreateAssetMenu(menuName = "Paper Dolls/Album Definition")]
    public sealed class AlbumDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField, TextArea] private string description;
        [SerializeField] private DollDefinition associatedDoll;
        [SerializeField] private Sprite albumCoverImage;
        [SerializeField] private List<ItemDefinition> catalogItems = new List<ItemDefinition>();
        [SerializeField] private GameObject bookPrefab;
        [SerializeField] private int totalPages = 20;
        [SerializeField] private List<ItemDefinition> starterItems = new List<ItemDefinition>();

        public string Id { get { return id; } }
        public string DisplayName { get { return displayName; } }
        public string Description { get { return description; } }
        public DollDefinition AssociatedDoll { get { return associatedDoll; } }
        public Sprite AlbumCoverImage { get { return albumCoverImage; } }
        public IReadOnlyList<ItemDefinition> CatalogItems { get { return catalogItems; } }
        public GameObject BookPrefab { get { return bookPrefab; } }
        public int TotalPages { get { return totalPages; } }
        public IReadOnlyList<ItemDefinition> StarterItems { get { return starterItems; } }
    }
}
