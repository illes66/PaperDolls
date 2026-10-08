using UnityEngine;

namespace PaperDollsGame.Content
{
    /// <summary>Loads bundled JSON TextAssets from Resources/Data (Assets/_Project/Resources/Data/*.json).</summary>
    public sealed class LocalContentProvider : IContentProvider
    {
        private const string ResourceFolder = "Data/";

        public string LoadJson(string contentName)
        {
            TextAsset asset = Resources.Load<TextAsset>(ResourceFolder + contentName);
            return asset != null ? asset.text : null;
        }
    }
}
