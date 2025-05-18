using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [System.Serializable]
    public class SeasonRuleData
    {
        public Season season;
        public FilterRule seasonRule;

        public void CopyFrom(SeasonRuleData source)
        {
            season = source.season;
            seasonRule = source.seasonRule;
        } // CopyFrom()
    } // class SeasonRuleData
} // namespace Magique.SoulLink

