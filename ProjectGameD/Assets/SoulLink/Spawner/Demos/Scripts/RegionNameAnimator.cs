using UnityEngine;
using UnityEngine.UI;

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class RegionNameAnimator : MonoBehaviour
    {
        public void SetRegionName(string regionName)
        {
            GetComponent<Text>().text = regionName;
            GetComponent<Animator>().SetTrigger("Reveal");
        } // SetRegionName()
    } // class RegionNameAnimator
} // namespace Magique.SoulLink
