namespace HHG.InventorySystem.Runtime
{
    public interface IInventoryDropHandler
    {
        bool CanHandleDrop(UIInventorySlot from, UIInventorySlot to) => true;
        void HandleDrop(UIInventorySlot from, UIInventorySlot to);
    }
}