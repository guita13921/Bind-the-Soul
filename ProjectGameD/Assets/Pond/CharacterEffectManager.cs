using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public class CharacterEffectManager : MonoBehaviour
    {
        public WeaponFX rightWeaponFX;
        public WeaponFX leftWeaponFX;

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

        public virtual void ActivateWeaponFX(bool isLeft)
        {
            if (isLeft == false)
            {
                if (rightWeaponFX != null)
                {
                    rightWeaponFX.trailEffect.active = true;

                }
            }
            else
            {
                if (leftWeaponFX != null)
                {
                    leftWeaponFX.trailEffect.active = true;
                }

            }
        }

        public virtual void DeactivateWeaponFX(bool isLeft)
        {
            if (isLeft == false)
            {
                if (rightWeaponFX != null)
                {
                    rightWeaponFX.trailEffect.active = false;

                }
            }
            else
            {
                if (leftWeaponFX != null)
                {
                    leftWeaponFX.trailEffect.active = false;
                }

            }
        }
    }
}