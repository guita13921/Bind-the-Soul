using System.Collections;
using System.Collections.Generic;
using TrailsFX;
using UnityEngine;

namespace SG
{
    public class WeaponFX : MonoBehaviour
    {
        [Header("Weapon FX")]
        public ParticleSystem normalVFX;
        public TrailEffect trailEffect;

        public void PlayWeaponNormalFX()
        {
            normalVFX.Stop();

            if (normalVFX.isStopped)
            {
                normalVFX.Play();
            }

        }

        public void PlayTrailVFX()
        {
            trailEffect.enabled = true;
            Invoke(nameof(StopTrailVFX), 1f); // Automatically calls StopTrailVFX after 1 second
        }

        public void StopTrailVFX()
        {
            trailEffect.enabled = false;
        }


    }
}