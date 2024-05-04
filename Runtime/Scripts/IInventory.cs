using System.Collections.Generic;

namespace HHG.InventorySystem.Runtime
{
    public interface IInventory : IEnumerable<IInventoryItem>
    {
        public int Count { get; }
        public IInventoryItem this[int i] { get; set; }
        public IReadOnlyList<IInventoryItem> Items { get; }
        public void Swap(int i, int j);
    }
}