using System.Collections;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class PlayerControl : MonoBehaviour
{
    bool GotHit =false;
    private void OnTriggerEnter(Collider Hit){
        if(Hit.gameObject.CompareTag("EnemyWeapon") && !animator.GetCurrentAnimatorStateInfo(0).IsName("GotHit")){
            animator.Play("GotHit", 0, 0);
            GotHit = true;
        }
    }


}
