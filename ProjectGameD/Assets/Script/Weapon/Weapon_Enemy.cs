using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Enemy : MonoBehaviour
{
    public float damage;
    public SFX sfx;
    public GameObject vfxPrefabs; // Array to hold references to VFX prefabs
    public Transform parentObject; // The object inside which you want to spawn the new object
    public GameObject blood; // Array to hold references to VFX prefabs


    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.gameObject.GetComponent<enemy>();
        if (enemy != null)
        {
                if(enemy.gameObject.CompareTag("Player")){
                sfx.Hit();
                Instantiate(vfxPrefabs,parentObject);
                //Instantiate(blood);


            Debug.Log(damage);
            enemy.health.currentHealth -= damage;}
        
            
        }
    }
}
