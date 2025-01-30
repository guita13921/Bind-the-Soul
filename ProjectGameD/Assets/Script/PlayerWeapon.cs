using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public float damage;
    float damageR = 0f;
    public CharacterData characterData;

    [SerializeField]
    bool isQK = true;

    void Start()
    {
        if (isQK && characterData.Q1_QKDamageUp)
            damage += 500;
        damageR = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        damage = damageR;

        if (isQK && characterData.Q2_QKCrit)
        {
            int randomValue = UnityEngine.Random.Range(0, 10); // Generate a random integer between 0 and 9
            if (randomValue < 2)
            {
                damage *= 3;
            }
        }
        var enemy = other.gameObject.GetComponent<enemy>();
        if (enemy != null)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.health.currentHealth -= damage;
            }
        }
    }
}
