using HHG.Common.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HHG.Inventory.Runtime
{
    [CreateAssetMenu(fileName = "Inventory", menuName = "HHG/Inventory System/Inventory Asset")]
    public class InventoryAsset : ScriptableObject
    {
        public IEnumerable<IInventoryItem> Items => items.Cast<IInventoryItem>();

        [SerializeField, Dropdown(typeof(IInventoryItem))] private List<ScriptableObject> items = new List<ScriptableObject>();

        public void SetItems(IEnumerable<IInventoryItem> enumerable)
        {
            items.Clear();
            items.AddRange(enumerable.Cast<ScriptableObject>());
        }
    }
}