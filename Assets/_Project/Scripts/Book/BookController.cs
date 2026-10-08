using System;
using System.Collections.Generic;
using PaperDollsGame.Content;
using UnityEngine;

namespace PaperDollsGame.Book
{
    public enum BookState
    {
        Closed,
        Open,
        Turning
    }

    // Maps a page to the items shown on it. Fill BookController.pages in the inspector
    // (one entry per page, e.g. 20), or call SetPages at runtime.
    [Serializable]
    public class PageContent
    {
        public int pageIndex;
        public List<ItemDefinition> itemsOnThisPage = new List<ItemDefinition>();
    }

    public sealed class BookController : MonoBehaviour
    {
        private static readonly int PageIndexHash = Animator.StringToHash("pageIndex");

        [SerializeField] private Animator animator;
        [SerializeField] private Transform itemsRoot;
        [SerializeField] private List<PageContent> pages = new List<PageContent>();
        [SerializeField] private bool openOnStart = true;

        private readonly Dictionary<int, List<GameObject>> spawnedItems = new Dictionary<int, List<GameObject>>();
        private int currentPage;
        private int targetPage;

        public BookState State { get; private set; } = BookState.Closed;
        public int CurrentPage { get { return currentPage; } }
        public int PageCount { get { return pages.Count; } }
        public bool CanGoNext { get { return State == BookState.Open && currentPage < pages.Count - 1; } }
        public bool CanGoPrev { get { return State == BookState.Open && currentPage > 0; } }

        // Raised whenever the visible page changes so OutfitSelectUI can show the available items.
        public event Action<int, IReadOnlyList<ItemDefinition>> PageChanged;
        public event Action<BookState> StateChanged;

        private void Start()
        {
            if (openOnStart)
            {
                Open();
            }
        }

        public void SetPages(List<PageContent> newPages)
        {
            ClearSpawnedItems();
            pages = newPages ?? new List<PageContent>();
            currentPage = 0;
            if (State != BookState.Closed)
            {
                RefreshPage();
            }
        }

        public void Open()
        {
            if (State != BookState.Closed)
            {
                return;
            }

            SetState(BookState.Open);
            RefreshPage();
        }

        public void Close()
        {
            if (State != BookState.Open)
            {
                return;
            }

            SetState(BookState.Closed);
            SetActiveItems(currentPage, false);
        }

        public void NextPage()
        {
            if (CanGoNext)
            {
                TurnTo(currentPage + 1);
            }
        }

        public void PrevPage()
        {
            if (CanGoPrev)
            {
                TurnTo(currentPage - 1);
            }
        }

        // Called by the last frame Animation Event of the page-turn clip.
        public void OnPageTurningComplete()
        {
            if (State != BookState.Turning)
            {
                return;
            }

            SetActiveItems(currentPage, false);
            currentPage = targetPage;
            SetState(BookState.Open);
            RefreshPage();
        }

        private void TurnTo(int page)
        {
            targetPage = page;
            SetState(BookState.Turning);

            if (animator != null)
            {
                animator.SetInteger(PageIndexHash, page);
            }
            else
            {
                OnPageTurningComplete();
            }
        }

        private void RefreshPage()
        {
            SetActiveItems(currentPage, true);
            if (animator != null)
            {
                animator.SetInteger(PageIndexHash, currentPage);
            }

            PageChanged?.Invoke(currentPage, GetItems(currentPage));
        }

        private void SetState(BookState state)
        {
            State = state;
            StateChanged?.Invoke(state);
        }

        private IReadOnlyList<ItemDefinition> GetItems(int page)
        {
            PageContent content = FindPage(page);
            return content != null ? content.itemsOnThisPage : (IReadOnlyList<ItemDefinition>)Array.Empty<ItemDefinition>();
        }

        private PageContent FindPage(int page)
        {
            for (int i = 0; i < pages.Count; i++)
            {
                if (pages[i] != null && pages[i].pageIndex == page)
                {
                    return pages[i];
                }
            }

            return null;
        }

        // Item visuals are instantiated lazily once per page, then toggled on/off.
        private void SetActiveItems(int page, bool active)
        {
            List<GameObject> objects;
            if (!spawnedItems.TryGetValue(page, out objects))
            {
                if (!active)
                {
                    return;
                }

                objects = new List<GameObject>();
                PageContent content = FindPage(page);
                if (content != null)
                {
                    Transform parent = itemsRoot != null ? itemsRoot : transform;
                    foreach (ItemDefinition item in content.itemsOnThisPage)
                    {
                        if (item != null && item.VisualPrefab != null)
                        {
                            objects.Add(Instantiate(item.VisualPrefab, parent));
                        }
                    }
                }

                spawnedItems[page] = objects;
            }

            foreach (GameObject go in objects)
            {
                if (go != null)
                {
                    go.SetActive(active);
                }
            }
        }

        private void ClearSpawnedItems()
        {
            foreach (List<GameObject> objects in spawnedItems.Values)
            {
                foreach (GameObject go in objects)
                {
                    if (go != null)
                    {
                        Destroy(go);
                    }
                }
            }

            spawnedItems.Clear();
        }
    }
}
