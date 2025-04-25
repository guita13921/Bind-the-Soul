using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public abstract class PowerUp : ScriptableObject
    {
        public string Name;
        public string Description;

        public abstract void Apply(PlayerData playerData);

        public abstract void Apply(PlayerStats playerStats);
    }

}