using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
        [CreateAssetMenu(menuName = "Items/Weapon Item")]
        public class WeaponItem : Item
        {
                public GameObject modelPrefab;
                public GameObject ThrowingModelPrefab;

                [Header("Weapon Type")]
                public WeaponType weaponType;
                public StantType stantType;

                [Header("Animator Replacer")]
                public AnimatorOverrideController weaponController;
                public String offHandIdleAniamtion = "Left_Arm_Idle_01";

                [Header("Idle Animations")]
                public string right_hand_idle;
                public string left_hand_idle;
                public string th_idle;

                [Header("Damage")]
                public int damage;
                public int criticalDamageMultiple = 4;

                [Header("Weapon Art")]
                public int weaponArtCharge;

                [Header("Modifiers")]
                public float lightAttackDamageModifiers;
                public float heavyAttackDamageModifiers;
                //string OH_Light_Attack_1 = "OH_Light_Attack_1";

                [Header("stamina Costs")]
                public int baseStamina;
                public float lightAttackStaminaMultiplier;
                public float heavyAttackStaminaMultiplier;
                public float WeaponArtStaminaMultiplier;

                [Header("Absorption")]
                public float physicalDamageAbsorption;
                public float staminaDamageModifier;
                public int ShieldPoint; //1 = 100% of weapon damage

                [Header("Sound FX")]
                public AudioClip[] weaponWhooshes;
        }
}

