using System;
using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace EnemyAI.GOAP.Behaviors
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(AgentBehaviour))]
    public class AgentMoveBehavior : MonoBehaviour
    {
        private NavMeshAgent NavMeshAgent;
        private Animator Animator;
        private AgentBehaviour AgentBehavior;
        private ITarget CurrentTarget;
        [SerializeField]private CapsuleCollider capsuleCollider;
        [SerializeField]private BoxCollider boxCollider;
        [SerializeField] private float MinMoveDistance = 0.25f;

        private Vector3 LastPosition;
        private static readonly int WALK = Animator.StringToHash("Walk");
        private static readonly int STUNT = Animator.StringToHash("Stunt");

        private void Awake()
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            AgentBehavior = GetComponent<AgentBehaviour>();
        }

        private void OnEnable()
        {
            AgentBehavior.Events.OnTargetChanged += EventsOnTargetChanged;
            AgentBehavior.Events.OnTargetOutOfRange += EventsOnTargetOutOfRange;
        }

        private void OnDisable()
        {
            AgentBehavior.Events.OnTargetChanged -= EventsOnTargetChanged;
            AgentBehavior.Events.OnTargetOutOfRange -= EventsOnTargetOutOfRange;
        }

        private void EventsOnTargetOutOfRange(ITarget target)
        {
            Animator.SetBool(WALK, false);
        }

        private void EventsOnTargetChanged(ITarget target, bool inRange)
        {
            CurrentTarget = target;
            LastPosition = CurrentTarget.Position;
            NavMeshAgent.SetDestination(target.Position);
            Animator.SetBool(WALK, true);
        }

        private void Update()
        {
            if (CurrentTarget == null)
            {
                return;
            }

            if (MinMoveDistance <= Vector3.Distance(CurrentTarget.Position, LastPosition))
            {
                LastPosition = CurrentTarget.Position;
                NavMeshAgent.SetDestination(CurrentTarget.Position);    
            }
            
            Animator.SetBool(WALK, NavMeshAgent.velocity.magnitude > 0.1f);
        }

        void EnableAttack(){
            boxCollider.enabled = true;
        }

        void DisableAttack(){
            boxCollider.enabled = false;
        }

        void OnTriggerEnter(Collider other){
            if(other.isTrigger && other.gameObject.CompareTag("PlayerSword")){
                Animator.SetBool(STUNT, true);
                boxCollider.enabled = false;
                print(other);
            }
        }
    }
}