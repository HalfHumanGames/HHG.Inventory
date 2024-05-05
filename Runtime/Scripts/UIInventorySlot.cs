using HHG.Common.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HHG.InventorySystem.Runtime
{
    public class UIInventorySlot : MonoBehaviour, IDropHandler
    {
        public IInventoryItem Item => inventoryItem;

        [SerializeField] private UIInventoryItem uiInventoryItem;

        private IInventoryItem inventoryItem;
        private InventoryController _controller;
        public InventoryController Controller
        {
            get
            {
                if (_controller == null)
                {
                    _controller = GetComponentInParent<InventoryController>();
                }
                return _controller;
            }
        }

        public void Refresh(IInventoryItem inventoryItem)
        {
            this.inventoryItem = inventoryItem;
            uiInventoryItem.Refresh(this.inventoryItem);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag.TryGetComponentInParent(out UIInventorySlot from))
            {
                Controller.HandleDrop(from, this);
            }
        }
    }
}