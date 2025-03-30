using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVFX : MonoBehaviour
{
    public ParticleSystem attackEffect;
    public ParticleSystem shieldEffect;
    public ParticleSystem damageEffect;
    public ParticleSystem shieldHitEffect;

    public void PlayAttackEffect()
    {
        if (attackEffect) attackEffect.Play();
    }

    public void PlayShieldEffect()
    {
        if (shieldEffect) shieldEffect.Play();
    }

    public void PlayDamageEffect()
    {
        if (damageEffect) damageEffect.Play();
    }

    public void PlayShieldHitEffect()
    {
        if (shieldHitEffect) shieldHitEffect.Play();
    }
}