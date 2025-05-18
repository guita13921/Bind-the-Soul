#if SURVIVAL_ENGINE || FARMING_ENGINE

#if SURVIVAL_ENGINE
using SurvivalEngine;
using UnityEngine;
#endif

#if FARMING_ENGINE
using FarmingEngine;
#endif

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class Spawnable_SurvivalEngine : Magique.SoulLink.Spawnable
    {
        UniqueID uniqueID;
        private Destructible _destructible;
        private AnimalWild _animalWild;

        override public void Awake()
        {
            base.Awake();
            uniqueID = GetComponent<UniqueID>();

            GetComponent<Selectable>().dont_destroy = true;

            _destructible = GetComponent<Destructible>();
            if (_destructible != null)
            {
                _destructible.onDeath += OnDeath;
            } // if

            _animalWild = GetComponent<AnimalWild>();
        } // Awake()

        override public void OnSpawned()
        {
            base.OnSpawned();

            GenerateUID();
        } // OnSpawned()

        override public void OnDespawned()
        {
            ClearUID();

            if (_animalWild != null)
            { 
                _animalWild.Reset();
            } // if

            base.OnDespawned();
        } // OnDespawned()

        /// <summary>
        /// Get a unique identifier for the life of this AI
        /// </summary>
        public void GenerateUID()
        {
            uniqueID.GenerateUID();
        } // GenerateUID()

        /// <summary>
        /// Clear the unique identifier when this AI is out of scope
        /// </summary>
        public void ClearUID()
        {
            uniqueID.SetUID("");
        } // ClearUID()
    } // Spawnable_SurvivalEngine
} // namespace Magique.SoulLink
#endif