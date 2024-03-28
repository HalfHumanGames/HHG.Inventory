using HHG.Common.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HHG.InventorySystem.Runtime
{
    public class UIInventorySlot : MonoBehaviour, IDropHandler
    {
        public IInventoryItem Item => item;

        [SerializeField] private UIInventoryItem uiInventoryItem;

        private IInventoryItem item;
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
            item = inventoryItem;
            uiInventoryItem.Refresh(item);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag.TryGetComponentInParent(out UIInventorySlot other))
            {
                int from = other.transform.GetSiblingIndex();
                int to = transform.GetSiblingIndex();

                if (Controller == other.Controller)
                {
                    Controller.SwapItems(from, to);
                }
                else
                {
                    IInventoryItem thisItem = Item;
                    IInventoryItem otherItem = other.Item;
                    Controller.SetItem(to, otherItem);
                    other.Controller.SetItem(from, thisItem);
                }
            }
        }
    }
}