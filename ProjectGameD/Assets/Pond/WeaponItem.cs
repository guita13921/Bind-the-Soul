using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItem : Item
    {
        public GameObject modelPrefab;
        public WeaponType weaponType;

        [Header("Idle Animations")]
        public string right_hand_idle;
        public string left_hand_idle;

        [Header("Damage")]
        public int damage;
        public int criticalDamageMultiple;

        [Header("One Handed Attack Animations")]
        public string OH_Light_Attack_1;
        public string OH_Light_Attack_2;
        public string OH_Light_Attack_3;
        public string OH_Light_Attack_4;
        public string OH_Heavy_Attack_1;

        [Header("stamina Costs")]
        public int baseStamina;
        public float lightAttackMultiplier;
        public float heavyAttackMultiplier;

        [Header("Absorption")]
        public float physicalDamageAbsorption;
        public int ShieldPoint;

    }
}

