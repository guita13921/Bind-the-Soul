using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public float damage;
    public CharacterData characterData;

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.gameObject.GetComponent<enemy>();
        if (enemy != null)
        {
            if (enemy.CompareTag("Enemy"))
                enemy.health.currentHealth -= damage;
        }
    }
}
