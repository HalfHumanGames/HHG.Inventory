using HHG.Common.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HHG.InventorySystem.Runtime
{
    [RequireComponent(typeof(UIInventory))]
    public class InventoryController : MonoBehaviour
    {
        public UIInventory UI => uiInventory;
        public IInventory Inventory => inventory;

        public UnityEvent<IInventory> OnUpdated;
        public UnityEvent<IInventory, IInventoryItem> OnItemAdded;
        public UnityEvent<IInventory, IInventoryItem> OnItemRemoved;

        [SerializeField] private StartAction startAction;
        [SerializeField] private InventoryAsset _inventory;

        private Lazy<UIInventory> _uiInventory = new Lazy<UIInventory>();
        private UIInventory uiInventory => _uiInventory.FromComponent(this);
        private IInventory inventory;
        private List<IInventoryDropHandler> handlers = new List<IInventoryDropHandler>() { new InventoryDropHandler() };

        private enum StartAction
        {
            Show,
            Hide
        }

        private void Awake()
        {
            inventory ??= _inventory == null ? new Inventory() : new Inventory(new List<IInventoryItem>(_inventory.Items));
            SetInventory(inventory);
        }

        private void Start()
        {
            uiInventory.Refresh(inventory);

            switch (startAction)
            {
                case StartAction.Show: uiInventory.Show(); break;
                case StartAction.Hide: uiInventory.Hide(); break;
            }
        }

        public void SetInventory(IInventory value)
        {
            Unsubscribe();
            inventory = value;
            Subscribe();
            uiInventory.Refresh(inventory);
        }

        public void AddHandler(IInventoryDropHandler handler)
        {
            handlers.Add(handler);
        }

        public void RemoveHandler<T>() where T : IInventoryDropHandler
        {
            handlers.Remove(h => h.GetType() == typeof(T));
        }

        public void HandleDrop(UIInventorySlot from, UIInventorySlot to)
        {
            for (int i = handlers.Count - 1; i >= 0; i--)
            {
                if (handlers[i].CanHandleDrop(from, to))
                {
                    handlers[i].HandleDrop(from, to);
                    break;
                }
            }
        }

        private void Subscribe()
        {
            if (inventory != null)
            {
                inventory.Updated += uiInventory.Refresh;
                inventory.Updated += OnUpdated.Invoke;
                inventory.ItemAdded += OnItemAdded.Invoke;
                inventory.ItemRemoved += OnItemRemoved.Invoke;
            }
        }

        private void Unsubscribe()
        {
            if (inventory != null)
            {
                inventory.Updated -= uiInventory.Refresh;
                inventory.Updated -= OnUpdated.Invoke;
                inventory.ItemAdded -= OnItemAdded.Invoke;
                inventory.ItemRemoved -= OnItemRemoved.Invoke;
            }
        }

        private void OnDestroy()
        {      
            Unsubscribe();
        }
    }
}