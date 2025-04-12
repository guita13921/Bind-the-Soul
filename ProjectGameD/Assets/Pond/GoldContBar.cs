using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{

    public class GoldContBar : MonoBehaviour
    {
        public Text goldCountText;

        public void SetGoldCountText(int goldCount)
        {
            goldCountText.text = goldCount.ToString();
        }
    }
}