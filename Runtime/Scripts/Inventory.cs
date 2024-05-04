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

        public IReadOnlyList<IInventoryItem> Items => items;

        private List<IInventoryItem> items;

        public Inventory(int size = 0)
        {
            items = new List<IInventoryItem>();
            items.Resize(size);
        }

        public Inventory(List<IInventoryItem> list, int size = -1)
        {
            items = list;
            if (size > items.Count)
            {
                items.Resize(size);
            }
        }

        public Inventory(IEnumerable<IInventoryItem> enumerable, int size = -1)
        {
            items = new List<IInventoryItem>(enumerable);
            if (size > items.Count)
            {
                items.Resize(size);
            }
        }

        public void Swap(int i, int j)
        {
            items.Resize(Mathf.Max(items.Count, i + 1, j + 1));
            items.Swap(i, j);
        }

        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

        public IEnumerator<IInventoryItem> GetEnumerator() => items.GetEnumerator();
    }
}