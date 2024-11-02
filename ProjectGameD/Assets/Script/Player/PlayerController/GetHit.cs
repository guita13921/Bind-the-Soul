using System.Collections;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class PlayerControl : MonoBehaviour
{
    bool GotHit = false;

    private void OnTriggerEnter(Collider Hit)
    {
        if (
            Hit.gameObject.CompareTag("EnemyWeapon")
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("GotHit")
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("die")
        )
        {
            GotHit = true;
            animator.Play("GotHit", 0, 0);
        }
    }

    private void Imframe()
    {
        Weapon_Enemy[] enemyWeapons = FindObjectsOfType<Weapon_Enemy>();
        foreach (Weapon_Enemy enemyWeapon in enemyWeapons)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("GotHit"))
            {
                Physics.IgnoreCollision(
                    GetComponent<CapsuleCollider>(),
                    enemyWeapon.GetComponent<Collider>(),
                    true
                );
            }
            else
            {
                Physics.IgnoreCollision(
                    GetComponent<CapsuleCollider>(),
                    enemyWeapon.GetComponent<Collider>(),
                    false
                );
            }
        }
    }
}
