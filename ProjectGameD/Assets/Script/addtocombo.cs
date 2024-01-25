using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addtocombo : MonoBehaviour
{
    [SerializeField] AttackSO Attack;
    private void OnTriggerEnter(Collider other) 
    {
     var player = other.GetComponent<PlayerCombat>();
     if(player != null){
        player.combo.Add(Attack);
        Destroy(gameObject);
     }


    }
}
