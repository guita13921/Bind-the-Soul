using UnityEngine;
using TMPro;

namespace SG
{
    public class TreasureDisplay : MonoBehaviour
    {
        public TreasureItem item;

        private TextMeshPro nameText;
        private TextMeshPro descriptionText;
        private Transform cameraTransform;

        public void SetText()
        {
            // Find child text objects
            nameText = transform.Find("Name").GetComponent<TextMeshPro>();
            descriptionText = transform.Find("Description").GetComponent<TextMeshPro>();

            if (item != null)
            {
                nameText.text = item.itemName;
                descriptionText.text = item.description;
            }

            // Find camera for billboard effect
            GameObject camObj = GameObject.Find("Camera02");
            if (camObj != null)
            {
                cameraTransform = camObj.transform;
            }
            else
            {
                Debug.LogError("Camera02 not found in scene!");
            }
        }

        void Update()
        {
            if (cameraTransform != null)
            {
                // Face camera
                nameText.transform.rotation = Quaternion.LookRotation(nameText.transform.position - cameraTransform.position);
                descriptionText.transform.rotation = Quaternion.LookRotation(descriptionText.transform.position - cameraTransform.position);
            }
        }
    }
}
