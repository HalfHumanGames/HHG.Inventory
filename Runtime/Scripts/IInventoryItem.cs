using HHG.Common.Runtime;
using UnityEngine;

namespace HHG.Inventory.Runtime
{
    public interface IInventoryItem : ITooltip
    {
        public Sprite Icon { get; }
        public Sprite Background { get; }
        public Color IconColor { get; }
        public Color BackgroundColor { get; }
    }
}