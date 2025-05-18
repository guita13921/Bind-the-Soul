using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [System.Serializable]
    public class GlobalSnowRuleData
    {
        public FilterRule filterRule;
        public float threshold = 0.5f;

        public void CopyFrom(GlobalSnowRuleData source)
        {
            filterRule = source.filterRule;
            threshold = source.threshold;
        } // CopyFrom()
    } // class GlobalSnowRuleData
} // namespace Magique.SoulLink

