using System.Collections;

namespace HHG.InventorySystem.Runtime
{
    public interface IInventory : IEnumerable
    {
        public int Count { get; }
        public IInventoryItem this[int i] { get; set; }
        public void Swap(int i, int j);
    }
}