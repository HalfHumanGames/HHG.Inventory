using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HHG.InventorySystem.Runtime
{
    [RequireComponent(typeof(UIInventory))]
    public class InventoryController : MonoBehaviour
    {
        public UIInventory UI => uiInventory;

        [SerializeField] private InventoryAsset _inventory;
        
        private UIInventory uiInventory;
        private IInventory inventory;

        public UnityEvent<IInventory> OnUpdated;

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

        public void SwapItems(int from, int to)
        {
            inventory.Swap(from, to);
            uiInventory.Refresh(inventory);
            OnUpdated?.Invoke(inventory);
        }

        public void SetItem(int index, IInventoryItem item)
        {
            inventory[index] = item;
            uiInventory.Refresh(inventory);
            OnUpdated?.Invoke(inventory);
        }

        public int AddItem(IInventoryItem item)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] == null)
                {
                    inventory[i] = item;
                    uiInventory.Refresh(inventory);
                    OnUpdated?.Invoke(inventory);
                    return i;
                }
            }

            return -1;
        }

        public bool TryAddItem(IInventoryItem item, out int index)
        {
            index = AddItem(item);
            return index >= 0;
        }
    }
}