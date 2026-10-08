namespace PaperDollsGame.Content
{
    /// <summary>Supplies raw JSON text for a content file ("events", "items", "albums").</summary>
    public interface IContentProvider
    {
        /// <summary>Returns the JSON text, or null if unavailable.</summary>
        string LoadJson(string contentName);
    }
}
