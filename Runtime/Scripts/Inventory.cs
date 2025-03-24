using HHG.Common.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Inventory.Runtime
{
    public class Inventory : IInventory
    {
        public int Count => items.Count;
        public bool IsReadOnly => false;
        public IEnumerable<IInventoryItem> Items => items.NotNull();

        public event Action<IInventory> Updated;
        public event Action<IInventory, IInventoryItem> ItemAdded;
        public event Action<IInventory, IInventoryItem> ItemRemoved;

        public IInventoryItem this[int i]
        {
            get => i < items.Count ? items[i] : null;
            set
            {
                items.Resize(Mathf.Max(items.Count, i + 1));

                IInventoryItem added = value;
                IInventoryItem removed = items[i];

                items[i] = value;

                if (added != null)
                {
                    ItemAdded?.Invoke(this, added);
                }

                if (removed != null)
                {
                    ItemRemoved?.Invoke(this, removed);
                }

                Updated?.Invoke(this);
            }
        }

        private List<IInventoryItem> items;

        public Inventory(int size = 0)
        {
            items = new List<IInventoryItem>();
            items.Resize(size);
        }

        public Inventory(List<IInventoryItem> list, int size = -1)
        {
            items = list;
            if (size > Count)
            {
                items.Resize(size);
            }
        }

        public Inventory(IEnumerable<IInventoryItem> enumerable, int size = -1)
        {
            items = new List<IInventoryItem>(enumerable);
            if (size > Count)
            {
                items.Resize(size);
            }
        }

        public void Set(IEnumerable<IInventoryItem> enumerable)
        {
            int i = 0;

            foreach (IInventoryItem item in enumerable)
            {
                this[i++] = item;
            }

            while (i < Count)
            {
                this[i] = null;
                items.RemoveAt(i);
            }
        }

        public void Swap(int i, int j)
        {
            IInventoryItem temp = this[i];
            this[i] = this[j];
            this[j] = temp;
        }

        public void Resize(int size)
        {
            items.Resize(size);
        }

        public bool TryAdd(IInventoryItem item, out int index)
        {
            if (item != null)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (this[i] == null)
                    {
                        this[i] = item;
                        index = i;
                        return true;
                    }
                }
            }

            index = -1;
            return false;
        }

        public int IndexOf(IInventoryItem item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, IInventoryItem item)
        {
            this[index] = item;
        }

        public void RemoveAt(int index)
        {
            this[index] = null;
        }

        public void Add(IInventoryItem item)
        {
            if (item != null)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (this[i] == null)
                    {
                        this[i] = item;
                        return;
                    }
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < Count; i++)
            {
                this[i] = null;
            }
        }

        public bool Contains(IInventoryItem item)
        {
            return items.Contains(item);
        }

        public void CopyTo(IInventoryItem[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public bool Remove(IInventoryItem item)
        {
            if (item != null)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (this[i] == item)
                    {
                        this[i] = null;
                        return true;
                    }
                }
            }

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

        public IEnumerator<IInventoryItem> GetEnumerator() => items.GetEnumerator();
    }
}