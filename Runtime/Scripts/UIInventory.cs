using HHG.Common.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HHG.InventorySystem.Runtime
{
    [ExecuteInEditMode, RequireComponent(typeof(CanvasGroup)), SelectionBase]
    public class UIInventory : MonoBehaviour, IRefreshable<IInventory>
    {
        public IReadOnlyList<UIInventorySlot> Slots => slots;

        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private Vector2Int slotSize;
        [SerializeField] private int padding;
        [SerializeField] private float spacing;
        [SerializeField] private int slotPadding;
        [SerializeField] private float slotSpacing;
        [SerializeField] private UIInventorySlot slotPrefab;
        [SerializeField] private GridLayoutGroup slotContainer;

        private CanvasGroup canvasGroup;
        private List<UIInventorySlot> slots = new List<UIInventorySlot>();
        private IRefreshable<IInventory>[] refreshables;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            refreshables = GetComponentsInChildren<IRefreshable<IInventory>>();

            if (Application.isPlaying)
            {
                CreateSlots();
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

            LayoutRebuilder.ForceRebuildLayoutImmediate(slotContainer.GetComponent<RectTransform>());
        }

        private void CreateSlots()
        {
            int count = gridSize.x * gridSize.y;
            slots = new List<UIInventorySlot>(count);

            if (!slotPrefab || !slotContainer) return;

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
                    slot.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy;
                }

                while (container.childCount > count)
                {
                    DestroyImmediate(container.GetChild(container.childCount - 1).gameObject);
                }
            }
        }
    }
}