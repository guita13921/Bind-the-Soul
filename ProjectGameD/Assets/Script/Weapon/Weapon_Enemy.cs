using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Enemy : MonoBehaviour
{
    public float damage;
    private void OnTriggerEnter(Collider other) {
        var player = other.gameObject.GetComponent<PlayerControl>();
        if(player != null){
            Debug.Log(damage); 
        }
    }
}
