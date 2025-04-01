using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public class PlayerEquipmentManager : MonoBehaviour
    {
        InputHander inputHandler;
        PlayerInventory playerInventory;
        BlockingCollider blockingCollider;

        void Awake()
        {
            inputHandler = GetComponentInParent<InputHander>();
            playerInventory = GetComponentInParent<PlayerInventory>();
            blockingCollider = GetComponentInChildren<BlockingCollider>();
        }

        public void OpenBlockCollider()
        {
            // 
            // IInputHander ....
            //
            blockingCollider.SetColliderDamageAbsorption(playerInventory.leftWeapon);
            blockingCollider.EnableBlockingCollider();
        }

        public void CloseBlockingCollider()
        {
            blockingCollider.DisableBlockingCollider();
        }
    }
}