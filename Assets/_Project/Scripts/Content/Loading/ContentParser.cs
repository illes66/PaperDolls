using System;
using System.Collections.Generic;
using UnityEngine;

namespace PaperDollsGame.Content
{
    /// <summary>Converts JSON text into runtime definition instances.</summary>
    public static class ContentParser
    {
        public static List<ItemDefinition> ParseItems(string json)
        {
            var result = new List<ItemDefinition>();
            ItemsFileJson file = Deserialize<ItemsFileJson>(json);
            if (file == null || file.items == null)
                return result;

            foreach (ItemJson dto in file.items)
            {
                RequireId(dto.id, "item");
                var item = ScriptableObject.CreateInstance<ItemDefinition>();
                item.name = dto.id;
                Sprite icon = string.IsNullOrEmpty(dto.iconPath) ? null : Resources.Load<Sprite>(dto.iconPath);
                GameObject prefab = string.IsNullOrEmpty(dto.prefabPath) ? null : Resources.Load<GameObject>(dto.prefabPath);
                item.Initialize(dto.id, dto.name, ParseSlot(dto.slot), ToList(dto.tags), icon, prefab);
                result.Add(item);
            }
            return result;
        }

        public static List<EventDefinition> ParseEvents(string json)
        {
            var result = new List<EventDefinition>();
            EventsFileJson file = Deserialize<EventsFileJson>(json);
            if (file == null || file.events == null)
                return result;

            foreach (EventJson dto in file.events)
            {
                RequireId(dto.id, "event");
                var tagRules = new List<TagScoreRule>();
                if (dto.tagRules != null)
                    foreach (TagRuleJson rule in dto.tagRules)
                        tagRules.Add(new TagScoreRule(rule.tag, rule.points));
                var slotRules = new List<SlotScoreRule>();
                if (dto.slotRules != null)
                    foreach (SlotRuleJson rule in dto.slotRules)
                        slotRules.Add(new SlotScoreRule(ParseSlot(rule.slot), rule.points));

                var evt = ScriptableObject.CreateInstance<EventDefinition>();
                evt.name = dto.id;
                string currency = dto.reward != null && !string.IsNullOrEmpty(dto.reward.currency) ? dto.reward.currency : "GEMS";
                int amount = dto.reward != null ? Mathf.Max(0, dto.reward.amount) : 0;
                evt.Initialize(dto.id, dto.name, dto.description, tagRules, slotRules, currency, amount);
                result.Add(evt);
            }
            return result;
        }

        public static List<AlbumDefinition> ParseAlbums(string json, IDictionary<string, ItemDefinition> itemsById)
        {
            var result = new List<AlbumDefinition>();
            AlbumsFileJson file = Deserialize<AlbumsFileJson>(json);
            if (file == null || file.albums == null)
                return result;

            foreach (AlbumJson dto in file.albums)
            {
                RequireId(dto.id, "album");
                var album = ScriptableObject.CreateInstance<AlbumDefinition>();
                album.name = dto.id;
                album.Initialize(
                    dto.id, dto.name, dto.description,
                    ResolveItems(dto.itemIds, itemsById, dto.id),
                    ResolveItems(dto.starterItemIds, itemsById, dto.id),
                    dto.totalPages > 0 ? dto.totalPages : 20);
                result.Add(album);
            }
            return result;
        }

        private static List<ItemDefinition> ResolveItems(string[] ids, IDictionary<string, ItemDefinition> itemsById, string albumId)
        {
            var list = new List<ItemDefinition>();
            if (ids == null)
                return list;
            foreach (string id in ids)
            {
                ItemDefinition item;
                if (itemsById != null && itemsById.TryGetValue(id, out item))
                    list.Add(item);
                else
                    Debug.LogWarning("Album '" + albumId + "' references unknown item '" + id + "'.");
            }
            return list;
        }

        private static T Deserialize<T>(string json) where T : class
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;
            return JsonUtility.FromJson<T>(json);
        }

        private static List<string> ToList(string[] values)
        {
            return values != null ? new List<string>(values) : new List<string>();
        }

        private static ItemSlot ParseSlot(string value)
        {
            ItemSlot slot;
            if (!Enum.TryParse(value, true, out slot) || !Enum.IsDefined(typeof(ItemSlot), slot))
                throw new FormatException("Unknown item slot '" + value + "'.");
            return slot;
        }

        private static void RequireId(string id, string kind)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new FormatException("A " + kind + " in the content JSON has an empty id.");
        }
    }
}
