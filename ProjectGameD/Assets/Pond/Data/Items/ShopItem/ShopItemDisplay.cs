using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SG
{

    public class ShopItemDisplay : MonoBehaviour
    {
        public TextMeshProUGUI itemNameText;
        public Image itemIcon;

        public void Setup(ShopItem item)
        {
            itemNameText.text = item.itemName;
            itemIcon.sprite = item.icon;
        }
    }
}