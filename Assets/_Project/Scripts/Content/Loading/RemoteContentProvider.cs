using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace PaperDollsGame.Content
{
    /// <summary>
    /// Downloads JSON from baseUrl + "/" + name + ".json" and caches it under persistentDataPath.
    /// Call <see cref="Download"/> (as a coroutine) to refresh; <see cref="LoadJson"/> serves the cache
    /// and falls back to the optional provider so the game still works offline.
    /// </summary>
    public sealed class RemoteContentProvider : IContentProvider
    {
        private readonly string baseUrl;
        private readonly string cacheDirectory;
        private readonly IContentProvider fallback;

        public RemoteContentProvider(string baseUrl, IContentProvider fallback = null, string cacheDirectory = null)
        {
            this.baseUrl = (baseUrl ?? string.Empty).TrimEnd('/');
            this.fallback = fallback;
            this.cacheDirectory = cacheDirectory ?? Path.Combine(Application.persistentDataPath, "ContentCache");
        }

        public IEnumerator Download(string contentName, Action<bool> onComplete = null)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(baseUrl + "/" + contentName + ".json"))
            {
                yield return request.SendWebRequest();
                bool ok = request.result == UnityWebRequest.Result.Success;
                if (ok)
                {
                    try
                    {
                        Directory.CreateDirectory(cacheDirectory);
                        File.WriteAllText(CachePath(contentName), request.downloadHandler.text);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning("Could not cache content '" + contentName + "': " + e.Message);
                        ok = false;
                    }
                }
                else
                {
                    Debug.LogWarning("Could not download content '" + contentName + "': " + request.error);
                }
                if (onComplete != null)
                    onComplete(ok);
            }
        }

        public string LoadJson(string contentName)
        {
            string path = CachePath(contentName);
            if (File.Exists(path))
                return File.ReadAllText(path);
            return fallback != null ? fallback.LoadJson(contentName) : null;
        }

        private string CachePath(string contentName)
        {
            return Path.Combine(cacheDirectory, contentName + ".json");
        }
    }
}
