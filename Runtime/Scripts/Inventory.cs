using HHG.Common.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.InventorySystem.Runtime
{
    public class Inventory : IInventory
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

        private List<IInventoryItem> items;

        public Inventory()
        {
            items = new List<IInventoryItem>();
        }

        public Inventory(List<IInventoryItem> list)
        {
            items = list;
        }

        public Inventory(IEnumerable<IInventoryItem> enumerable) : this()
        {
            items.AddRange(enumerable);
        }

        public void Swap(int i, int j)
        {
            items.Resize(Mathf.Max(items.Count, i + 1, j + 1));
            items.Swap(i, j);
        }

        public IEnumerator GetEnumerator() => items.GetEnumerator();
    }
}