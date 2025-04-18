using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "ShopItem")]

    public class ShopItem : ScriptableObject
    {
        [Header("Item Info")]
        public string itemName;

        [TextArea]
        public string description;
        public Sprite icon;

        [Header("Item Pricing")]
        public int cost;

        public GameObject itemPrefab; // For 3D/usable item, optional
    }

}