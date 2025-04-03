using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public class CharacterEffectManager : MonoBehaviour
    {
        public WeaponFX rightWeaponFX;
        public WeaponFX leftWeaponFX;

        EnemyAnimatorManager enemyAnimatorManager;

        public virtual void PlayWeaponFX(bool isLeft)
        {
            if (isLeft == false)
            {
                if (rightWeaponFX != null)
                {
                    rightWeaponFX.PlayTrailVFX();
                }
            }
            else
            {
                if (leftWeaponFX != null)
                {
                    leftWeaponFX.PlayTrailVFX();
                }

            }
        }

        public virtual void StopWeaponFX(bool isLeft)
        {
            if (isLeft == false)
            {
                if (rightWeaponFX != null)
                {
                    rightWeaponFX.StopTrailVFX();
                }
            }
            else
            {
                if (leftWeaponFX != null)
                {
                    leftWeaponFX.StopTrailVFX();
                }

            }
        }
    }
}