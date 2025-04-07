using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public class SpellItem : Item
    {

        public GameObject SpellWarmUpFx;
        public GameObject spellCastFX;
        public string spellAnimation;

        [Header("Spell Type")]
        public string Typename;

        [Header("Spell Description")]
        [TextArea]
        public string SpellDescription;

        public virtual void AttemptToCastSpell(
            AnimatorManager animatorManager,
            CharacterStats character,
            WeaponSlotManager weaponSlotManager
        )
        {
            Debug.Log("Successfully AttemptToCastSpell");
        }

        public virtual void SuccessfullyCastSpell(
            AnimatorManager animatorManager,
            CharacterStats character,
            CameraHandler cameraHandler,
            EnemyWeaponSlotManager enemyWeaponSlotManager
        )
        {
            Debug.Log("Successfully CastSpell");
        }
    }

}