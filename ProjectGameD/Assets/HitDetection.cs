using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    public GameObject hitVFX;
    public GameObject dmgtext;
    public GameObject[] sound;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerSword"))
        {
            GameObject vfx = Instantiate(hitVFX, this.transform.position, Quaternion.identity);

            PlayerWeapon playerWeapon = other.GetComponent<PlayerWeapon>();
            if (sound != null && sound.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, sound.Length);
                GameObject soundObject = sound[randomIndex];
                GameObject swordhit = Instantiate(soundObject);
                AudioSource audioSource = soundObject.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }
            if (playerWeapon != null)
            {
                float damage = playerWeapon.damage;
                Debug.Log(damage);
                Vector3 newPosition = this.transform.position;
                newPosition.y += 1;
                GameObject dmg = Instantiate(dmgtext, newPosition, Quaternion.Euler(0, 60, 0));

                damageShow damageTextComponent = dmg.GetComponent<damageShow>();
                if (damageTextComponent != null)
                {
                    damageTextComponent.SetDamage(damage);
                }
            }
        }
    }
}
