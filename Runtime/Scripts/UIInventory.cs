using HHG.Common.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HHG.InventorySystem.Runtime
{
    [ExecuteInEditMode, RequireComponent(typeof(CanvasRenderer), typeof(CanvasGroup)), SelectionBase]
    public class UIInventory : MonoBehaviour, IRefreshable<IInventory>
    {
        public IReadOnlyList<UIInventorySlot> Slots => slots;

        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private Vector2Int viewSize;
        [SerializeField] private Vector2Int slotSize;
        [SerializeField] private int padding;
        [SerializeField] private float spacing;
        [SerializeField] private int slotPadding;
        [SerializeField] private float slotSpacing;
        [SerializeField] private UIInventorySlot slotPrefab;
        [SerializeField] private GridLayoutGroup slotContainer;

        private Lazy<CanvasGroup> _canvasGroup = new Lazy<CanvasGroup>();
        private CanvasGroup canvasGroup => _canvasGroup.FromComponent(this);
        private Lazy<RectTransform> _rectTransform = new Lazy<RectTransform>();
        private RectTransform rectTransform => _rectTransform.FromComponent(this);
        private List<UIInventorySlot> slots = new List<UIInventorySlot>();
        private Lazy<IRefreshable<IInventory>> _refreshables = new Lazy<IRefreshable<IInventory>>();
        private IRefreshable<IInventory>[] refreshables => _refreshables.FromComponentsInChildren(this);

        private void Awake()
        {
            if (Application.isPlaying)
            {
                CreateSlots(true);
            }
        }

        public void Show(bool show)
        {
            if (show)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        public void Show()
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void Select()
        {
            if (slots.FirstOrDefault() is UIInventorySlot firstSlot)
            {
                firstSlot.Selectable.Select();
            }
        }

        private void Update()
        {
            if (!Application.isPlaying) UpdateEditor();
        }

        private void UpdateEditor()
        {
            CreateSlots();
        }

        public void Refresh(IInventory inventory)
        {
            foreach (IRefreshable<IInventory> refreshable in refreshables)
            {
                if (refreshable != this as IRefreshable<IInventory>)
                {
                    refreshable.Refresh(inventory);
                }
            }

            for (int i = 0; i < slots.Count; i++)
            {
                if (i < inventory.Count)
                {
                    slots[i].Refresh(inventory[i]);
                }
                else
                {
                    slots[i].Refresh(null);
                }
            }
        }

        public void Resize(Vector2Int size, int max)
        {
            slotContainer.constraintCount = size.x;
            Transform container = slotContainer.transform;

            while (slots.Count < max)
            {
                UIInventorySlot slot = Instantiate(slotPrefab, container);
                slot.Refresh(null);
                slots.Add(slot);
            }

            while (slots.Count > max)
            {
                int last = slots.Count - 1;
                UIInventorySlot slot = slots[last];
                slots.RemoveAt(last);
                Destroy(slot.gameObject);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        private void CreateSlots(bool force = false)
        {
            if (!force && (!slotPrefab || !slotContainer)) return;

            int count = gridSize.x * gridSize.y;
            slots ??= new List<UIInventorySlot>(count);
            slots.Capacity = count; // In case already created
            slotContainer.cellSize = slotSize;
            slotContainer.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            slotContainer.constraintCount = gridSize.x;
            slotContainer.padding = new RectOffset(slotPadding, slotPadding, slotPadding, slotPadding);
            slotContainer.spacing = new Vector2(slotSpacing, slotSpacing);

            VerticalLayoutGroup verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.padding = new RectOffset(padding, padding, padding, padding);
            verticalLayoutGroup.spacing = spacing;

            Transform container = slotContainer.transform;

            if (Application.isPlaying)
            {
                for (int i = 0; i < slots.Capacity; i++)
                {
                    slots.Add(Instantiate(slotPrefab, container));
                }
            }
            else
            {
                while (container.childCount < count)
                {
                    UIInventorySlot slot = Instantiate(slotPrefab, container);
                    slot.gameObject.hideFlags = HideFlags.DontSaveInEditor;
                }

                while (container.childCount > count)
                {
                    DestroyImmediate(container.GetChild(container.childCount - 1).gameObject);
                }
            }

            if (this.TryGetComponentInChildren(out ScrollRect scroll))
            {
                if (viewSize.x <= 0)
                {
                    viewSize.x = gridSize.x;
                }

                if (viewSize.y <= 0)
                {
                    viewSize.y = gridSize.y;
                }

                RectTransform rect = scroll.GetComponent<RectTransform>();

                Scrollbar hbar = scroll.horizontalScrollbar;
                Scrollbar vbar = scroll.verticalScrollbar;

                RectTransform hRect = hbar.GetComponent<RectTransform>();
                RectTransform vRect = vbar.GetComponent<RectTransform>();

                float addx = viewSize.y == gridSize.y ? 0f : vRect.sizeDelta.x;
                float addy = viewSize.x == gridSize.x ? 0f : hRect.sizeDelta.y;

                rect.sizeDelta = new Vector2(
                    (viewSize.x * slotSize.x) + ((viewSize.x - 1) * slotSpacing) + (slotPadding * 2) + addx,
                    (viewSize.y * slotSize.y) + ((viewSize.y - 1) * slotSpacing) + (slotPadding * 2) + addy
                );
            }

            slots.Select(s => s.Selectable).SetNavigationGrid(gridSize.x);

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }
}