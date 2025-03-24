namespace HHG.Inventory.Runtime
{
    public class InventoryDropHandler : IInventoryDropHandler
    {
        public void HandleDrop(UIInventorySlot from, UIInventorySlot to)
        {
            if (from.Controller == to.Controller)
            {
                from.Controller.Inventory.Swap(from.Index, to.Index);
            }
            else
            {
                // Can't pass directly since SetItem updates the slot
                IInventoryItem fromItem = from.Item;
                IInventoryItem toItem = to.Item;
                from.Controller.Inventory[from.Index] = toItem;
                to.Controller.Inventory[to.Index] = fromItem;
            }
        }
    }
}