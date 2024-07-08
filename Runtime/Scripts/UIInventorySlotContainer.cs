using HHG.Common.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HHG.InventorySystem.Runtime
{
    public class UIInventorySlotContainer : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private Lazy<InventoryController> controller = new Lazy<InventoryController>();
        public InventoryController Controller => controller.FromComponentInParent(this);

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerDrag.TryGetComponent(out UIInventoryItem item))
            {
                UIInventorySlot from = item.Slot;
                UIInventorySlot to = Controller.UI.Slots.MinBy(s => Vector3.Distance(s.transform.position, from.transform.position));

                if (from != to)
                {
                    Controller.HandleDragEnter(from, to);
                }
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag.TryGetComponent(out UIInventoryItem item))
            {
                UIInventorySlot from = item.Slot;
                UIInventorySlot to = Controller.UI.Slots.MinBy(s => Vector3.Distance(s.transform.position, from.transform.position));

                if (from != to)
                {
                    Controller.HandleDrop(from, to);
                }              
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag && eventData.pointerDrag.TryGetComponent(out UIInventoryItem item))
            {
                UIInventorySlot from = item.Slot;
                UIInventorySlot to = Controller.UI.Slots.MinBy(s => Vector3.Distance(s.transform.position, from.transform.position));

                if (from != to)
                {
                    Controller.HandleDragEnter(from, to);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerDrag && eventData.pointerDrag.TryGetComponent(out UIInventoryItem item))
            {
                UIInventorySlot from = item.Slot;
                UIInventorySlot to = Controller.UI.Slots.MinBy(s => Vector3.Distance(s.transform.position, from.transform.position));

                if (from != to)
                {
                    Controller.HandleDragExit(from, to);
                }
            }
        }
    }
}