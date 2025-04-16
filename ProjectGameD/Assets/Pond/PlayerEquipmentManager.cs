using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public class PlayerEquipmentManager : MonoBehaviour
    {
        InputHander inputHandler;
        PlayerInventory playerInventory;
        public BlockingColliderPlayer blockingCollider;

        void Awake()
        {
            inputHandler = GetComponentInParent<InputHander>();
            playerInventory = GetComponentInParent<PlayerInventory>();
        }

        public void OpenBlockingCollider()
        {
            // 
            //if(inputHandle.twoHandFlag)
            /*{
                blockingCollider.SetColliderDamageAbsorption(playerInventory.rightWeapon);
            }
            else
            {
                blockingCollider.SetColliderDamageAbsorption(playerInventory.leftWeapon);
            }*/
            //
            // blockingCollider.SetColliderDamageAbsorption(playerInventory.leftWeapon); 
            blockingCollider.EnableBlockingCollider();
        }

        public void CloseBlockingCollider()
        {
            blockingCollider.DisableBlockingCollider();
        }
    }
}