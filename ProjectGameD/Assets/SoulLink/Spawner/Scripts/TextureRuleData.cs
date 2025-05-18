using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [System.Serializable]
    public class TextureRuleData
    {
        public int textureIndex;
        public FilterRule textureRule;
        public float threshold = 0.5f;

        public void CopyFrom(TextureRuleData source)
        {
            textureIndex = source.textureIndex;
            textureRule = source.textureRule;
            threshold = source.threshold;
        } // CopyFrom()
    } // class TextureRuleData
} // namespace Magique.SoulLink

