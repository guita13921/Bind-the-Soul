using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SG
{
    public class ShopManager : MonoBehaviour
    {
        private List<ShopItem> availableShopItems = new List<ShopItem>();
        List<ShopItemDisplay> shopItemDisplay;

        private void Start()
        {
            // Load all shop items from Resources
            availableShopItems = Resources.LoadAll<ShopItem>("ShopItems").ToList();
            shopItemDisplay = GetComponentsInChildren<ShopItemDisplay>().ToList();

            if (availableShopItems.Count == 0)
            {
                Debug.LogWarning("No ShopItems found in Resources/ShopItems.");
                return;
            }

            LoadShopItems();
        }

        private void LoadShopItems()
        {
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

            List<ShopItem> shuffledItems = availableShopItems.OrderBy(i => Random.value).ToList();

            for (int i = 0; i < holders.Count; i++)
            {
                Transform holder = holders[i];
                ShopItem itemToUse = i < shuffledItems.Count
                    ? shuffledItems[i]
                    : availableShopItems[Random.Range(0, availableShopItems.Count)];

                if (itemToUse.itemPrefab != null)
                {
                    GameObject instance = Instantiate(itemToUse.itemPrefab, holder);
                    instance.transform.localPosition = Vector3.zero;


                    // ✅ Get the ShopItemDisplay component from the holder
                    ShopItemDisplay display = holder.GetComponent<ShopItemDisplay>();

                    if (display != null)
                    {
                        display.item = itemToUse;
                        display.SetText(); // Make sure you have this method in ShopItemDisplay
                    }
                    else
                    {
                        Debug.LogWarning("ShopItemHolder is missing ShopItemDisplay component.");
                    }
                }
                else
                {
                    Debug.LogWarning($"ShopItem '{itemToUse.name}' has no prefab assigned.");
                }
            }
        }

    }
}
