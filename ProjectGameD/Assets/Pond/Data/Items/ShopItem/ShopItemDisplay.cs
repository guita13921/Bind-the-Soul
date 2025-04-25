using UnityEngine;
using TMPro;
using Unity.VisualScripting;

namespace SG
{

    public class ShopItemDisplay : MonoBehaviour
    {
        public ShopItem item;

        private TextMeshPro nameText;
        private TextMeshPro costText;
        private Transform cameraTransform;

        private PlayerStats playerStats; // ✅ Added reference

        public void SetText()
        {
            // Find child texts
            nameText = transform.Find("Name").GetComponent<TextMeshPro>();
            costText = transform.Find("Cost").GetComponent<TextMeshPro>();


            // Assign item values
            if (item != null)
            {
                nameText.text = item.itemName;
                costText.text = item.cost.ToString();
            }

            // Grab camera02 transform
            GameObject camObj = GameObject.Find("Camera02");
            if (camObj != null)
            {
                cameraTransform = camObj.transform;
            }
            else
            {
                Debug.LogError("camera02 not found in scene!");
            }

            // ✅ Find PlayerStats component from GameObject tagged "Player"
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerStats = playerObj.GetComponent<PlayerStats>();
            }
            else
            {
                Debug.LogError("Player GameObject with tag 'Player' not found.");
            }
        }

        void Update()
        {
            if (cameraTransform != null)
            {
                // Make the nameText face the camera
                nameText.transform.rotation = Quaternion.LookRotation(nameText.transform.position - cameraTransform.position);
            }

            // ✅ Use playerStats to determine if player can afford item
            if (item != null && costText != null && playerStats != null)
            {
                if (playerStats.goldCount >= item.cost)
                    costText.color = Color.white;
                else
                    costText.color = Color.red;
            }
        }
    }
}