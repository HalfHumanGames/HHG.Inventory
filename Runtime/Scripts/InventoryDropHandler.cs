namespace HHG.InventorySystem.Runtime
{
    public class InventoryDropHandler : IInventoryDropHandler
    {
        public void HandleDrop(UIInventorySlot from, UIInventorySlot to)
        {
            int fromIndex = from.transform.GetSiblingIndex();
            int toIndex = to.transform.GetSiblingIndex();

            if (from.Controller == to.Controller)
            {
                from.Controller.SwapItems(fromIndex, toIndex);
            }
            else
            {
                // Can't pass directly since SetItem updates the slot
                IInventoryItem fromItem = from.Item;
                IInventoryItem toItem = to.Item;
                from.Controller.SetItem(fromIndex, toItem);
                to.Controller.SetItem(toIndex, fromItem);
            }
        }
    }
}