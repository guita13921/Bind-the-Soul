using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SG
{
    public class ShopManager : MonoBehaviour
    {
        private List<ShopItem> availableShopItems = new List<ShopItem>();

        private void Start()
        {
            // Load all shop items from Resources
            availableShopItems = Resources.LoadAll<ShopItem>("ShopItems").ToList();

            if (availableShopItems.Count == 0)
            {
                Debug.LogWarning("No ShopItems found in Resources/ShopItems.");
                return;
            }

            LoadShopItems();
        }

        private void LoadShopItems()
        {
            // Find all children (recursive) named "ShopItemHolder"
            List<Transform> holders = new List<Transform>();

            foreach (Transform child in GetComponentsInChildren<Transform>())
            {
                if (child.name == "ShopItemHolder")
                {
                    holders.Add(child);
                }
            }

            if (holders.Count == 0)
            {
                Debug.LogWarning("No ShopItemHolder children found under ShopManager.");
                return;
            }

            // Shuffle shop items if you want to assign unique items per holder
            List<ShopItem> shuffledItems = availableShopItems.OrderBy(i => Random.value).ToList();

            for (int i = 0; i < holders.Count; i++)
            {
                Transform holder = holders[i];

                // Choose an item: unique or allow repeat
                ShopItem itemToUse;

                if (i < shuffledItems.Count)
                {
                    itemToUse = shuffledItems[i]; // Unique assignment
                }
                else
                {
                    // If more holders than items, start repeating randomly
                    itemToUse = availableShopItems[Random.Range(0, availableShopItems.Count)];
                }

                if (itemToUse.itemPrefab != null)
                {
                    GameObject instance = Instantiate(itemToUse.itemPrefab, holder);
                    instance.transform.localPosition = Vector3.zero; // Optional: tweak for spacing
                }
                else
                {
                    Debug.LogWarning($"ShopItem '{itemToUse.name}' has no prefab assigned.");
                }
            }
        }
    }
}
