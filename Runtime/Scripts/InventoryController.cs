using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HHG.InventorySystem.Runtime
{
    [RequireComponent(typeof(UIInventory))]
    public class InventoryController : MonoBehaviour
    {
        public UIInventory UI => uiInventory;
        public IReadOnlyList<IInventoryItem> Items => inventory.Items;

        [SerializeField] private InventoryAsset _inventory;
        
        private UIInventory uiInventory;
        private IInventory inventory;
        private List<IInventoryDropHandler> dropHandlers = new List<IInventoryDropHandler>() { new InventoryDropHandler() };

        public UnityEvent<IInventory> OnUpdated;
        public UnityEvent<IInventory, IInventoryItem> OnItemAdded;
        public UnityEvent<IInventory, IInventoryItem> OnItemRemoved;

        private void Awake()
        {
            uiInventory = GetComponent<UIInventory>();
            inventory = _inventory == null ? new Inventory() : new Inventory(new List<IInventoryItem>(_inventory.Items));
        }

        private void Start()
        {
            uiInventory.Refresh(inventory);
        }

        public void SetInventory(IInventory value)
        {
            inventory = value;
            uiInventory.Refresh(inventory);
        }

        public void AddHandler(IInventoryDropHandler handler)
        {
            dropHandlers.Add(handler);
        }

        public void HandleDrop(UIInventorySlot from,  UIInventorySlot to)
        {
            for (int i = dropHandlers.Count - 1; i >= 0; i--)
            {
                if (dropHandlers[i].CanHandleDrop(from, to))
                {
                    dropHandlers[i].HandleDrop(from, to);
                    break;
                }
            }
        }

        public void SwapItems(int from, int to)
        {
            inventory.Swap(from, to);
            uiInventory.Refresh(inventory);
            OnUpdated?.Invoke(inventory);
        }

        public void SetItem(int index, IInventoryItem item)
        {
            IInventoryItem removed = inventory[index];
            IInventoryItem added = item;
            inventory[index] = item;
            uiInventory.Refresh(inventory);
            OnUpdated?.Invoke(inventory);
            if (removed != null)
            {
                OnItemRemoved?.Invoke(inventory, removed);
            }
            if (added != null)
            {
                OnItemAdded?.Invoke(inventory, added);
            }
        }

        public void AddItems(IEnumerable<IInventoryItem> items)
        {
            foreach (IInventoryItem item in items)
            {
                AddItem(item);
            }
        }

        public int AddItem(IInventoryItem item)
        {
            if (item != null)
            {
                for (int i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i] == null)
                    {
                        inventory[i] = item;
                        uiInventory.Refresh(inventory);
                        OnUpdated?.Invoke(inventory);
                        OnItemAdded?.Invoke(inventory, item);
                        return i;
                    }
                }
            }

            return -1;
        }

        public bool TryAddItem(IInventoryItem item, out int index)
        {
            index = AddItem(item);
            return index >= 0;
        }

        public void RemoveItems(IEnumerable<IInventoryItem> items)
        {
            foreach (IInventoryItem item in items)
            {
                RemoveItem(item);
            }
        }

        public bool RemoveItem(IInventoryItem item)
        {
            if (item != null)
            {
                for (int i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i] == item)
                    {
                        inventory[i] = null;
                        uiInventory.Refresh(inventory);
                        OnUpdated?.Invoke(inventory);
                        OnItemRemoved?.Invoke(inventory, item);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}