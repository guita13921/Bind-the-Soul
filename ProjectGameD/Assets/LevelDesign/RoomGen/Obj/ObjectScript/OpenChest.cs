using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class OpenChest : Interactable
    {

        public override void Interact(PlayerManager playerManager)
        {
            //Rotate our player towards the chest
            //lock his transform to certain point infront of the chest
            //open the chest lid, and animate the player
            //Spawn an item inside the chest the player can pick up

            Vector3 rotationDireation = transform.position - playerManager.transform.position;
            rotationDireation.y = 0;
            rotationDireation.Normalize();

            Quaternion tr = Quaternion.LookRotation(rotationDireation);
            Quaternion targetRotaion = Quaternion.Slerp(playerManager.transform.rotation, tr, 300 * Time.deltaTime);
            playerManager.transform.rotation = targetRotaion;
        }


    }

}