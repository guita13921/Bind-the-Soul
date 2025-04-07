using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Spells/Projectile Spell")]
    public class ProjectileSpell : SpellItem
    {
        [Header("Projectile Damage")]
        public float baseDamge;

        [Header("Projectile Physic")]
        public float projectileForwardVelocity;
        public float projectileUpwardVelocity;
        public float projectileMass;
        public bool isEffectedByGravity;
        Rigidbody rigidbody;

        public override void AttemptToCastSpell(
            AnimatorManager animatorManager,
            CharacterStats character,
            WeaponSlotManager weaponSlotManager
        )
        {
            base.AttemptToCastSpell(animatorManager, character, weaponSlotManager);
            GameObject instantiatedWarnUpSpellFX = Instantiate(SpellWarmUpFx, weaponSlotManager.rightHandSlot.transform);
            instantiatedWarnUpSpellFX.gameObject.transform.localScale = new Vector3(100, 100, 100);
            animatorManager.PlayTargetAnimation(spellAnimation, true);

        }

        public override void SuccessfullyCastSpell(
            AnimatorManager animatorManager,
            CharacterStats character,
            CameraHandler cameraHandler,
            EnemyWeaponSlotManager enemyWeaponSlotManager
        )
        {
            base.SuccessfullyCastSpell(animatorManager, character, cameraHandler, enemyWeaponSlotManager);

            GameObject instantiatedSpellFX = Instantiate(spellCastFX, enemyWeaponSlotManager.rightHandSlot.transform.position, Quaternion.identity);
            rigidbody = instantiatedSpellFX.GetComponent<Rigidbody>();
            Vector3 directionToCharacter = (character.transform.position - instantiatedSpellFX.transform.position).normalized;
            Vector3 launchVelocity = directionToCharacter * projectileForwardVelocity + Vector3.up * projectileUpwardVelocity;
            rigidbody.AddForce(launchVelocity, ForceMode.Impulse);
            rigidbody.useGravity = isEffectedByGravity;
            rigidbody.mass = projectileMass;
        }

    }
}