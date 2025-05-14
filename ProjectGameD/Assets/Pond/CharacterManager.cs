using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
namespace SG
{
    public class CharacterManager : MonoBehaviour
    {
        public CharacterSoundFXManager characterSoundFXManager;
        public EnemyWeaponSlotManager enemyWeaponSlotManager;
        public WeaponSlotManager weaponSlotManager;
        public CharacterCombatManager characterCombatManager;

        [Header("Lock On Transform")]
        public Transform lockOnTransform;
        public Transform DefaultlockOnTransform;

        [Header("Combat Colliders")]
        public CriticalDamageCollider backStabCollider;
        public CriticalDamageCollider riposteCollider;

        //[Header("Combat Colliders")]

        [Header("Combat Flags")]
        public bool CanDoCombo;
        public bool canBeRiposted;

        //public bool canBeParried;
        public bool isParrying;
        public bool isBlocking;
        public bool isInvulnerable;
        public bool isUsingRightHand;
        public bool isUsingLefthand;

        [Header("Movement Flags")]
        public bool isRotatingWithRootMotion;
        public bool canRotate;
        public bool isSprinting;
        public bool isInAir;
        public bool isGrounded;

        [Header("Spells")]
        public bool isFiringSpell;

        //Damage will be inflicted during an animation event
        //Used in backstab or riposte animations
        public int pendingCriticalDamage;

        protected virtual void Awake()
        {
            characterCombatManager = GetComponentInChildren<CharacterCombatManager>();
            characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
            enemyWeaponSlotManager = GetComponent<EnemyWeaponSlotManager>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        }
    }
}