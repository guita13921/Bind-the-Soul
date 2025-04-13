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

        bool hasCollider = false;
        Rigidbody rigidbody;

        Vector3 impactNormal;

        PlayerStats spellTarget;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
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
                    Destroy(impactParticle, 5f);
                    Destroy(gameObject, 5f);
                }
            }
            else if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Enemy")
            {
                //Debug.Log("Enemy");

                impactParticle = Instantiate(impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal));
                Destroy(projectileParticle);
                Destroy(impactParticle, 5f);
                Destroy(gameObject, 5f);

            }
        }
    }
}
