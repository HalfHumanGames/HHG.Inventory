using HHG.Common.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HHG.InventorySystem.Runtime
{
    public class UIInventorySlot : MonoBehaviour, IRefreshable<IInventoryItem>, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public IInventoryItem Item => inventoryItem;
        public InventoryController Controller => controller.FromComponentInParent(this);
        public T ItemAs<T>() where T : class, IInventoryItem => inventoryItem as T;
        public int Index => transform.GetSiblingIndex();

        private IInventoryItem inventoryItem;
        private Lazy<InventoryController> controller = new Lazy<InventoryController>();
        private IRefreshable<IInventoryItem>[] refreshables;

        private void Awake()
        {
            refreshables = GetComponentsInChildren<IRefreshable<IInventoryItem>>();
        }

        public void Refresh(IInventoryItem inventoryItem)
        {
            this.inventoryItem = inventoryItem;

            foreach (IRefreshable<IInventoryItem> refreshable in refreshables)
            {
                if (refreshable != this as IRefreshable<IInventoryItem>)
                {
                    refreshable.Refresh(inventoryItem);
                }
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag.TryGetComponent(out UIInventoryItem item))
            {
                UIInventorySlot from = item.Slot;

                if (from != this)
                {
                    Controller.HandleDrop(from, this);
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag && eventData.pointerDrag.TryGetComponent(out UIInventoryItem item))
            {
                UIInventorySlot from = item.Slot;

                if (from != this)
                {
                    Controller.HandleDragEnter(from, this);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerDrag && eventData.pointerDrag.TryGetComponent(out UIInventoryItem item))
            {
                UIInventorySlot from = item.Slot;

                if (from != this)
                {
                    Controller.HandleDragExit(from, this);
                }
            }
        }
    }
}