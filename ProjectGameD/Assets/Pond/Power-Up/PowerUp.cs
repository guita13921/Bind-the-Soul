using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public abstract class PowerUp : ScriptableObject
    {
        public string Name;
        public string Description;
        public SetName setName;

        public int powerUpLevel = 1;
        public PowerUp originalAsset; // used to track the source ScriptableObject

        public abstract void Apply(PlayerData playerData);
        public abstract void Apply(PlayerStats playerStats);

        public virtual void Apply2SetBonus(PlayerData data, PlayerStats stats) { }
        public virtual void Apply4SetBonus(PlayerData data, PlayerStats stats) { }
        public virtual void ApplyCurse(PlayerData data, PlayerStats stats) { }

        public virtual void OnStacked(PlayerData playerData) { } // Optional: trigger effects on level up
    }



}