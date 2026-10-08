using System;

namespace PaperDollsGame.Content
{
    // JSON DTOs (JsonUtility-compatible). Enums are written as strings and parsed by ContentParser.
    [Serializable] public sealed class TagRuleJson { public string tag; public int points; }
    [Serializable] public sealed class SlotRuleJson { public string slot; public int points; }
    [Serializable] public sealed class RewardJson { public string currency; public int amount; }

    [Serializable]
    public sealed class EventJson
    {
        public string id;
        public string name;
        public string description;
        public string[] tags;
        public TagRuleJson[] tagRules;
        public SlotRuleJson[] slotRules;
        public RewardJson reward;
    }

    [Serializable]
    public sealed class ItemJson
    {
        public string id;
        public string name;
        public string slot;
        public string[] tags;
        public string iconPath;
        public string prefabPath;
    }

    [Serializable]
    public sealed class AlbumJson
    {
        public string id;
        public string name;
        public string description;
        public string[] itemIds;
        public string[] starterItemIds;
        public int totalPages;
    }

    [Serializable] public sealed class EventsFileJson { public int version; public EventJson[] events; }
    [Serializable] public sealed class ItemsFileJson { public int version; public ItemJson[] items; }
    [Serializable] public sealed class AlbumsFileJson { public int version; public AlbumJson[] albums; }
}
