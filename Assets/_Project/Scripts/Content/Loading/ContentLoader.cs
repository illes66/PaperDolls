using System;
using System.Collections.Generic;
using UnityEngine;

namespace PaperDollsGame.Content
{
    /// <summary>
    /// Global access point for JSON-driven content. Swap the provider with <see cref="SetProvider"/>
    /// (local bundled data by default, e.g. <see cref="RemoteContentProvider"/> for LiveOps).
    /// </summary>
    public sealed class ContentLoader : MonoBehaviour
    {
        private static ContentLoader instance;

        private IContentProvider provider = new LocalContentProvider();
        private readonly Dictionary<string, EventDefinition> events = new Dictionary<string, EventDefinition>(StringComparer.Ordinal);
        private readonly Dictionary<string, ItemDefinition> items = new Dictionary<string, ItemDefinition>(StringComparer.Ordinal);
        private readonly Dictionary<string, AlbumDefinition> albums = new Dictionary<string, AlbumDefinition>(StringComparer.Ordinal);

        public static ContentLoader Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<ContentLoader>();
                    if (instance == null)
                        instance = new GameObject("ContentLoader").AddComponent<ContentLoader>();
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SetProvider(IContentProvider newProvider)
        {
            if (newProvider == null)
                throw new ArgumentNullException("newProvider");
            provider = newProvider;
            events.Clear();
            items.Clear();
            albums.Clear();
        }

        public List<ItemDefinition> LoadItems()
        {
            List<ItemDefinition> loaded = ContentParser.ParseItems(provider.LoadJson("items"));
            Replace(items, loaded, d => d.Id);
            return loaded;
        }

        public List<EventDefinition> LoadEvents()
        {
            List<EventDefinition> loaded = ContentParser.ParseEvents(provider.LoadJson("events"));
            Replace(events, loaded, d => d.Id);
            return loaded;
        }

        public List<AlbumDefinition> LoadAlbums()
        {
            if (items.Count == 0)
                LoadItems();
            List<AlbumDefinition> loaded = ContentParser.ParseAlbums(provider.LoadJson("albums"), items);
            Replace(albums, loaded, d => d.Id);
            return loaded;
        }

        public EventDefinition GetEvent(string id)
        {
            if (events.Count == 0)
                LoadEvents();
            return Get(events, id, "event");
        }

        public ItemDefinition GetItem(string id)
        {
            if (items.Count == 0)
                LoadItems();
            return Get(items, id, "item");
        }

        public AlbumDefinition GetAlbum(string id)
        {
            if (albums.Count == 0)
                LoadAlbums();
            return Get(albums, id, "album");
        }

        private static T Get<T>(Dictionary<string, T> lookup, string id, string kind)
        {
            T value;
            if (string.IsNullOrEmpty(id) || !lookup.TryGetValue(id, out value))
                throw new KeyNotFoundException("No " + kind + " definition exists for ID '" + id + "'.");
            return value;
        }

        private static void Replace<T>(Dictionary<string, T> lookup, List<T> loaded, Func<T, string> getId)
        {
            lookup.Clear();
            foreach (T definition in loaded)
            {
                string id = getId(definition);
                if (lookup.ContainsKey(id))
                    throw new InvalidOperationException("Duplicate content ID '" + id + "'.");
                lookup.Add(id, definition);
            }
        }
    }
}
