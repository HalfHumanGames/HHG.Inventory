using HHG.Common.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HHG.InventorySystem.Runtime
{
    [CreateAssetMenu(fileName = "Inventory", menuName = "HHG/Inventory System/Inventory Asset")]
    public class InventoryAsset : ScriptableObject
    {
        public IEnumerable<IInventoryItem> Items => items.Cast<IInventoryItem>();

        [SerializeField, Dropdown(typeof(IInventoryItem))] private List<ScriptableObject> items = new List<ScriptableObject>();
    }
}