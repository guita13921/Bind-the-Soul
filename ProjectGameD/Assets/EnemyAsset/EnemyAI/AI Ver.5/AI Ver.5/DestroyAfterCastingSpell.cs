using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public class DestroyAfterCastingSpell : MonoBehaviour
    {
        CharacterManager characterCastingSpell;

        void Awake()
        {
            characterCastingSpell = GetComponentInParent<CharacterManager>();
        }

        private void Update()
        {
            if (characterCastingSpell.isFiringSpell)
            {
                Destroy(gameObject);
            }
        }
    }
}