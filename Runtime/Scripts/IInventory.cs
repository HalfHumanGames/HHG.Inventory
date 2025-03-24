using System;
using System.Collections.Generic;

namespace HHG.Inventory.Runtime
{
    public interface IInventory : IList<IInventoryItem>
    {
        IEnumerable<IInventoryItem> Items { get; }
        void Set(IEnumerable<IInventoryItem> items);
        void Swap(int i, int j);
        void Resize(int size);
        bool TryAdd(IInventoryItem item, out int i);
        event Action<IInventory> Updated;
        event Action<IInventory, IInventoryItem> ItemAdded;
        event Action<IInventory, IInventoryItem> ItemRemoved;
    }

    public static class IInventoryExtensions
    {
        public static bool TryAdd(this IInventory inventory, IInventoryItem item)
        {
            return inventory.TryAdd(item, out _);
        }
    }
}