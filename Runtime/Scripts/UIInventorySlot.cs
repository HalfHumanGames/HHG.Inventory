using HHG.Common.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HHG.InventorySystem.Runtime
{
    [RequireComponent(typeof(Selectable), typeof(EventTrigger))]
    public class UIInventorySlot : MonoBehaviour, IRefreshable<IInventoryItem>, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public IInventoryItem Item => inventoryItem;
        public InventoryController Controller => controller;
        public T ItemAs<T>() where T : class, IInventoryItem => inventoryItem as T;
        public int Index => transform.GetSiblingIndex();
        public Selectable Selectable => selectable;
        public EventTrigger EventTrigger => eventTrigger;

        [SerializeField] private Vector2 gamepadDragOffset;

        private UIInventoryItem uiInventoryItem;
        private Selectable selectable;
        private EventTrigger eventTrigger;
        private Lazy<InventoryController> _controller = new Lazy<InventoryController>();
        private InventoryController controller => _controller.FromComponentInParent(this);
        private IInventoryItem inventoryItem;
        private IRefreshable<IInventoryItem>[] refreshables;

        private void Awake()
        {
            uiInventoryItem = GetComponentInChildren<UIInventoryItem>(true);
            selectable = GetComponent<Selectable>();
            eventTrigger = GetComponent<EventTrigger>();
            refreshables = GetComponentsInChildren<IRefreshable<IInventoryItem>>(true);

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Select;
            entry.callback.AddListener(OnSelect);
            eventTrigger.triggers.Add(entry);

            EventTrigger.Entry entry2 = new EventTrigger.Entry();
            entry2.eventID = EventTriggerType.Deselect;
            entry2.callback.AddListener(OnDeselect);
            eventTrigger.triggers.Add(entry2);
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

        private void OnSelect(BaseEventData eventData)
        {
            if (UIInventoryItem.Dragged)
            {
                if (uiInventoryItem.TryGetComponent(out TooltipTrigger tooltipTrigger))
                {
                    tooltipTrigger.HideTooltip();
                }

                UIInventoryItem.Dragged.Rect.position = transform.position;
                UIInventoryItem.Dragged.Rect.anchoredPosition += gamepadDragOffset;
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.pointerDrag = UIInventoryItem.Dragged.gameObject;
                OnPointerEnter(pointerEventData);
            }
        }

        private void OnDeselect(BaseEventData eventData)
        {
            if (UIInventoryItem.Dragged)
            {
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.pointerDrag = UIInventoryItem.Dragged.gameObject;
                OnPointerExit(pointerEventData);
            }
        }
    }
}