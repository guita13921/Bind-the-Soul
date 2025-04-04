using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SG
{
    public class EnemyLocomotionManager : MonoBehaviour
    {

        EnemyManager enemyManager;
        EnemyAnimatorManager enemyAnimatorManager;

        public CapsuleCollider CharacterCollider;
        public CapsuleCollider CharacterCollisiomBlockerCollider;

        public LayerMask detectopnLayer;

        private void Awake()
        {
            enemyManager = GetComponent<EnemyManager>();
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        }

        void Start()
        {
            Physics.IgnoreCollision(CharacterCollider, CharacterCollisiomBlockerCollider, true);
        }
    }
}