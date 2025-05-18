using System.Collections.Generic;
using UnityEngine;

namespace Magique
{
    public class Inventory : MonoBehaviour
    {
        private List<string> _inventory = new List<string>();

        public void AddItem(string itemName)
        {
            if (_inventory.Contains(itemName)) return;

            _inventory.Add(itemName);
        } // AddItem()

        public void RemoveItem(string itemName)
        {
            if (!_inventory.Contains(itemName)) return;

            _inventory.Remove(itemName);
        } // RemoveItem()

        public bool HasItem(string itemName)
        {
            return _inventory.Contains(itemName);
        } // HasItem()
    } // class Inventory
} // namespace Magique