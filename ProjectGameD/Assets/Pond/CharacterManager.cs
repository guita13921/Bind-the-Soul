using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SG
{
    public class CharacterManager : MonoBehaviour
    {
        [Header("Lock On Transform")]
        public Transform lockOnTransform;

        [Header("Combat Colliders")]

        [Header("Combat Flags")]
        public bool CanDoCombo;
        public bool canBeRiposted;
        public bool canBeParried;
        public bool isParrying;
        public bool isBlocking;
        public bool isInvulnerable;

        [Header("Movement Flags")]
        public bool isRotatingWithRootMotion;
        public bool canRotate;


        //Damage will be inflicted during an animation event
        //Used in backstab or riposte animations
        public int pendingCriticalDamage;
    }
}