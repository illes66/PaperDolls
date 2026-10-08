#if UNITY_EDITOR
using System.IO;
using PaperDollsGame.Content;
using UnityEditor;
using UnityEngine;

public static class SampleAlbumGenerator
{
    private const string Folder = "Assets/_Project/Content/Samples";
    private static readonly DollStyle[] Styles =
    {
        DollStyle.Classic, DollStyle.Modern, DollStyle.Casual,
        DollStyle.Formal, DollStyle.Playful, DollStyle.Casual
    };

    [MenuItem("Paper Dolls/Create Sample Albums (6)")]
    public static void Create()
    {
        Directory.CreateDirectory(Folder);
        for (int i = 1; i <= 6; i++)
        {
            string suffix = i.ToString("000");
            DollDefinition doll = CreateAsset<DollDefinition>(Folder + "/Doll_" + suffix + ".asset");
            Set(doll, "id", "doll_" + suffix);
            Set(doll, "displayName", "Sample Doll " + i);
            Set(doll, "description", "Placeholder doll " + i);
            SetEnum(doll, "style", (int)Styles[i - 1]);

            AlbumDefinition album = CreateAsset<AlbumDefinition>(Folder + "/Album_" + suffix + ".asset");
            Set(album, "id", "album_" + suffix);
            Set(album, "displayName", "Sample Album " + i);
            Set(album, "description", "Placeholder album " + i);
            var so = new SerializedObject(album);
            so.FindProperty("associatedDoll").objectReferenceValue = doll;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static T CreateAsset<T>(string path) where T : ScriptableObject
    {
        T existing = AssetDatabase.LoadAssetAtPath<T>(path);
        if (existing != null)
            return existing;
        T asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        return asset;
    }

    private static void Set(Object target, string field, string value)
    {
        var so = new SerializedObject(target);
        so.FindProperty(field).stringValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetEnum(Object target, string field, int value)
    {
        var so = new SerializedObject(target);
        so.FindProperty(field).enumValueIndex = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }
}
#endif
