using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace SG
{
    public class SpellDamageCollider : DamageCollider
    {
        public GameObject impactParticle;
        public GameObject projectileParticle;
        public GameObject muzzleParticle;

        public AudioClip hitSoundEffect; // 👈 Add this to assign the hit sound in Inspector
        private AudioSource audioSource; // 👈 We’ll use this to play the sound

        bool hasCollider = false;
        Rigidbody rigidbody;

        Vector3 impactNormal;

        PlayerStats spellTarget;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();

            // Setup AudioSource if one doesn't already exist
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.playOnAwake = false;
        }

        void Start()
        {
            projectileParticle = Instantiate(projectileParticle, transform.position, transform.rotation);
            projectileParticle.transform.parent = transform;

            if (muzzleParticle)
            {
                muzzleParticle = Instantiate(muzzleParticle, transform.position, transform.rotation);
                Destroy(muzzleParticle, 2f);
            }
        }

        void Update()
        {
            if (rigidbody.velocity != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(rigidbody.velocity);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            impactNormal = collision.contacts[0].normal; // Get the impact normal

            // Play hit sound effect
            if (hitSoundEffect != null)
            {
                audioSource.PlayOneShot(hitSoundEffect);
            }

            if (collision.gameObject.tag == "Player")
            {
                if (!hasCollider)
                {
                    spellTarget = collision.transform.GetComponent<PlayerStats>();

                    if (spellTarget != null)
                    {
                        spellTarget.TakeDamage(currentDamageWeapon);
                        Debug.Log("player");
                    }

                    hasCollider = true;
                    impactParticle = Instantiate(impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal));

                    Destroy(projectileParticle);
                    Destroy(impactParticle, 1f);
                    Destroy(gameObject, 1f);
                }
            }
            else if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "CantDash")
            {
                impactParticle = Instantiate(impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal));

                Destroy(projectileParticle);
                Destroy(impactParticle, 1f);
                Destroy(gameObject, 1f);
            }
        }
    }
}
