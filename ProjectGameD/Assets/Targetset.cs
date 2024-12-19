using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetset : MonoBehaviour
{
    public Vector3 targetOffset = Vector3.zero; // Offset to aim (can be set in the Inspector)
    private HS_TargetProjectile hS_TargetProjectile; // Internal reference to the projectile

    public void callprojectile(HS_TargetProjectile projectile)
    {
        if (projectile != null)
        {
            hS_TargetProjectile = projectile;
            hS_TargetProjectile.UpdateTarget(this.transform, targetOffset);
        }
        else
        {
            Debug.LogWarning("Projectile script is null!");
        }
    }
}
