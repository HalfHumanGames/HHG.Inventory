using HHG.Common.Runtime;
using UnityEngine;

namespace HHG.InventorySystem.Runtime
{
    public interface IInventoryItem : ITooltip
    {
        public Sprite Icon { get; }
    }
}