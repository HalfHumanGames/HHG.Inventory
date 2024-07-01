namespace HHG.InventorySystem.Runtime
{
    public interface IInventoryDropHandler
    {
        bool IsValidDropTarget(UIInventorySlot from, UIInventorySlot to) => true;
        bool CanDrop(UIInventorySlot from, UIInventorySlot to) => true;
        void HandleDrop(UIInventorySlot from, UIInventorySlot to);
        void HandleDragEnter(UIInventorySlot from, UIInventorySlot to) { }
        void HandleDragExit(UIInventorySlot from, UIInventorySlot to) { }
    }
}