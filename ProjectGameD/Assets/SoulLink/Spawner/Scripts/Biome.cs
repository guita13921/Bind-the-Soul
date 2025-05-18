using UnityEngine;

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class Biome : MonoBehaviour, IBiome
    {
        public string BiomeName
        {
            get { return _biomeName; }
            set { _biomeName = value; }
        }

        [SerializeField]
        private string _biomeName;

        virtual public void Initialize()
        {
        } // Initialize()

        virtual public bool IsInBiome(Vector3 spawnPos)
        {
            return false;
        } // IsInBiome()
    } // class Biome
} // namespace Magique.SoulLink
