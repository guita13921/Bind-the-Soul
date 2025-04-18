using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SG
{
    public class TreasureRoomManager : MonoBehaviour
    {
        private List<TreasureItem> availableTreasureItems = new List<TreasureItem>();
        private List<TreasureDisplay> treasureDisplays;

        private void Start()
        {
            // Load all treasure items from Resources/Treasure
            availableTreasureItems = Resources.LoadAll<TreasureItem>("Treasure").ToList();
            treasureDisplays = GetComponentsInChildren<TreasureDisplay>().ToList();


            if (availableTreasureItems.Count == 0)
            {
                Debug.LogWarning("No ShopItems found in Resources/Treasure.");
                return;
            }

            LoadTreasureItems();
        }

        private void LoadTreasureItems()
        {
            List<Transform> holders = new List<Transform>();

            foreach (Transform child in GetComponentsInChildren<Transform>())
            {
                if (child.name == "TreasureHolder")
                {
                    holders.Add(child);
                }
            }

            if (holders.Count == 0)
            {
                Debug.LogWarning("No TreasureHolder children found under TreasureRoomManager.");
                return;
            }

            List<TreasureItem> shuffledItems = availableTreasureItems.OrderBy(i => Random.value).ToList();

            for (int i = 0; i < holders.Count; i++)
            {
                Transform holder = holders[i];
                TreasureItem itemToUse = i < shuffledItems.Count
                    ? shuffledItems[i]
                    : availableTreasureItems[Random.Range(0, availableTreasureItems.Count)];

                if (itemToUse.itemPrefab != null)
                {
                    GameObject instance = Instantiate(itemToUse.itemPrefab, holder);
                    instance.transform.localPosition = Vector3.zero;

                    // ✅ Get the TreasureDisplay component from the holder
                    TreasureDisplay display = holder.GetComponent<TreasureDisplay>();

                    if (display != null)
                    {
                        display.item = itemToUse;
                        display.SetText(); // Make sure you implement this method in TreasureDisplay
                    }
                    else
                    {
                        Debug.LogWarning("TreasureHolder is missing TreasureDisplay component.");
                    }
                }
                else
                {
                    Debug.LogWarning($"TreasureItem '{itemToUse.name}' has no prefab assigned.");
                }
            }
        }
    }
}
