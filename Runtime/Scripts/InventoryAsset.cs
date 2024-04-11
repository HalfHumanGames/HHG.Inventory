using HHG.Common.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HHG.InventorySystem.Runtime
{
    [CreateAssetMenu(fileName = "Inventory", menuName = "HHG/Inventory System/Inventory Asset")]
    public class InventoryAsset : ScriptableObject, IInventory, ISerializationCallbackReceiver
    {
        public int Count => items.Count;

        public IInventoryItem this[int i]
        {
            get => i < items.Count ? items[i] : null;
            set
            {
                items.Resize(Mathf.Max(items.Count, i + 1));
                items[i] = value;
            }
        }

        public IReadOnlyList<IInventoryItem> Items => items;

        [SerializeField, Filter(typeof(IInventoryItem))] private List<ScriptableObject> _items = new List<ScriptableObject>();

        private List<IInventoryItem> items = new List<IInventoryItem>();

        public void Swap(int i, int j)
        {
            items.Resize(Mathf.Max(items.Count, i + 1, j + 1));
            items.Swap(i, j);
        }

        public void OnAfterDeserialize()
        {
            items.Clear();
            items.AddRange(_items.Select(i => i as IInventoryItem));
        }

        public void OnBeforeSerialize()
        {
            _items.Clear();
            _items.AddRange(items.Select(i => i as ScriptableObject));
        }

        public IEnumerator GetEnumerator() => items.GetEnumerator();
    }
}