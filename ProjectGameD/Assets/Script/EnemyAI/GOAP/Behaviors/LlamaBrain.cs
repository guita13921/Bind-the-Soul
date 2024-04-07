using CrashKonijn.Goap.Behaviours;
using EnemyAI.GOAP.Config;
using EnemyAI.GOAP.Goals;
using EnemyAI.Sensors;
using UnityEngine;

namespace EnemyAI.GOAP.Behaviors
{
    [RequireComponent(typeof(AgentBehaviour))]
    public class LlamaBrain : MonoBehaviour
    {
        [SerializeField] private PlayerSensor PlayerSensor;
        //[SerializeField] private HungerBehavior Hunger;
        [SerializeField] private AttackConfigSO AttackConfig;
        //[SerializeField] private BioSignsSO BioSigns;
        private AgentBehaviour AgentBehavior;
        private bool PlayerIsInRange;

        private void Awake()
        {
            AgentBehavior = GetComponent<AgentBehaviour>();
        }
        
        private void Start()
        {
            AgentBehavior.SetGoal<WanderGoal>(false);
            PlayerSensor.Collider.radius = AttackConfig.SensorRadius;
        }

        private void Update()
        {
            SetGoal();
        }

        private void SetGoal(){
        }

        private void OnEnable()
        {
            PlayerSensor.OnPlayerEnter += PlayerSensorOnPlayerEnter;
            PlayerSensor.OnPlayerExit += PlayerSensorOnPlayerExit;
        }

        private void OnDisable()
        {
            PlayerSensor.OnPlayerEnter -= PlayerSensorOnPlayerEnter;
            PlayerSensor.OnPlayerExit -= PlayerSensorOnPlayerExit;
        }

        private void PlayerSensorOnPlayerExit(Vector3 lastKnownPosition)
        {
            
            AgentBehavior.SetGoal<WanderGoal>(true);
        }

        private void PlayerSensorOnPlayerEnter(Transform Player)
        {
            AgentBehavior.SetGoal<KillPlayer>(true);
        }
    }
}