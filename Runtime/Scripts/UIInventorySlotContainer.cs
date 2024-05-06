using HHG.Common.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HHG.InventorySystem.Runtime
{
    public class UIInventorySlotContainer : MonoBehaviour, IDropHandler
    {
        private Lazy<InventoryController> controller = new Lazy<InventoryController>();
        public InventoryController Controller => controller.FromComponentInParent(this);

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag.TryGetComponentInParent(out UIInventorySlot from))
            {
                UIInventorySlot to = Controller.UI.Slots.MinBy(s => Vector3.Distance(s.transform.position, from.transform.position));

                if (from != to)
                {
                    Controller.HandleDrop(from, to);
                }              
            }
        }
    }
}