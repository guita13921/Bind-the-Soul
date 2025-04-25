using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "TreasureItem")]
    public class TreasureItem : ScriptableObject
    {
        [Header("Treasure Info")]
        public string itemName;

        [TextArea]
        public string description;

        public Sprite icon;

        [Header("Visual/Prefab")]
        public GameObject itemPrefab; // Optional 3D or UI object
    }
}
