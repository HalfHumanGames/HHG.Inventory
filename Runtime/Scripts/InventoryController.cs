using HHG.Common.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
        private Dictionary<IInventoryDropHandler, Texture2D[]> cursors = new Dictionary<IInventoryDropHandler, Texture2D[]>();

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

        public void RemoveHandler(IInventoryDropHandler handler)
        {
            handlers.Remove(handler);
        }

        public void RemoveHandler<T>() where T : IInventoryDropHandler
        {
            handlers.Remove(h => h.GetType() == typeof(T));
        }

        public void SetHandlerCursor<T>(Texture2D cursor, Texture2D cursor2 = null) where T : IInventoryDropHandler
        {
            T handler = handlers.OfType<T>().FirstOrDefault();
            cursors[handler] = cursor2 == null ?
                new Texture2D[] { cursor, cursor } : 
                new Texture2D[] { cursor, cursor2 };
        }

        public void UnsetHandlerCursor<T>() where T : IInventoryDropHandler
        {
            T handler = handlers.OfType<T>().FirstOrDefault();
            cursors.Remove(handler);
        }

        public void HandleDrop(UIInventorySlot from, UIInventorySlot to)
        {
            for (int i = handlers.Count - 1; i >= 0; i--)
            {
                if (handlers[i].IsValidDropTarget(from, to))
                {
                    if (handlers[i].CanDrop(from, to))
                    {
                        handlers[i].HandleDrop(from, to);
                    }
                    break;
                }
            }

            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }


        public void HandleDragEnter(UIInventorySlot from, UIInventorySlot to)
        {
            for (int i = handlers.Count - 1; i >= 0; i--)
            {
                if (handlers[i].IsValidDropTarget(from, to))
                {
                    if (cursors.TryGetValue(handlers[i], out Texture2D[] current))
                    {
                        Texture2D cursor = handlers[i].CanDrop(from, to) ? current[0] : current[1];
                        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
                    }
                    handlers[i].HandleDragEnter(from, to);
                    break;
                }
            }
        }

        public void HandleDragExit(UIInventorySlot from, UIInventorySlot to)
        {
            for (int i = handlers.Count - 1; i >= 0; i--)
            {
                if (handlers[i].IsValidDropTarget(from, to))
                {
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    handlers[i].HandleDragExit(from, to);
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