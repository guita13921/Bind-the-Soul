using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{

    public class GoldContBar : MonoBehaviour
    {
        public TextMeshProUGUI goldCountText;

        public void SetGoldCountText(int goldCount)
        {
            goldCountText.text = goldCount.ToString();
        }
    }
}