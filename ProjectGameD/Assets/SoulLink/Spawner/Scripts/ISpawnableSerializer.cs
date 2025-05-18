using System.Collections.Generic;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public interface ISpawnableSerializer
    {
        List<string> SerializeData();
        void DeserializeData(ExtraAIData extraAIData);
    } // interface ISpawnableSerializer
} // namespace Magique.SoulLink