using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public class PlayerEffectManager : CharacterEffectManager
    {

        AnimatorHander animatorHander;

        private void Awake()
        {
            animatorHander = GetComponentInChildren<AnimatorHander>();
        }

        void Update()
        {
            CheckisAttacking();
        }

        void CheckisAttacking()
        {
            if (animatorHander.anim.GetBool("isUsingRightHand") == true)
            {
                ActivateWeaponFX(false);
            }
            else
            {
                DeactivateWeaponFX(false);
            }


            if (animatorHander.anim.GetBool("isUsingLefthand") == true)
            {
                ActivateWeaponFX(true);
            }
            else
            {
                DeactivateWeaponFX(true);
            }

        }
    }
}