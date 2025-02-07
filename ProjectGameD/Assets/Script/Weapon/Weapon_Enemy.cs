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

    void Start()
    {
        gameObject.tag = "EnemyWeapon";
    }

    private void OnTriggerEnter(Collider other)
    { 
        if (other.gameObject.CompareTag("Player"))
        {
            var HP = other.gameObject.GetComponent<Health>();
            HP.currentHealth -= damage;
        }
    }

    private void OnTriggerStay (Collider other)
    { 
        if (other.gameObject.CompareTag("Player"))
        {
            var HP = other.gameObject.GetComponent<Health>();
            Debug.Log("OnTriggerStay");
            HP.currentHealth -= damage;
        }
    }
}
