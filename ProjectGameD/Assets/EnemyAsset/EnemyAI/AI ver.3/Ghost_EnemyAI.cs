using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Ghost_EnemyAI : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    NavMeshAgent agent;

    [SerializeField]
    CapsuleCollider caps;

    [SerializeField]
    LayerMask groundLayer,
        playerLayer;

    //Walk Var
    Vector3 destPoint;
    bool walkpointSet;

    [SerializeField]
    float range;

    [SerializeField]
    float sightRange,
        attackRange,
        RetreatRange;

    [SerializeField]
    bool playerInsight,
        PlayerInAttackrange,
        PlayerInRetreatRange;

    //State Var
    public enum State
    {
        Ready,
        Cooldown,
        KnockBack,
        Dead,
    };

    [SerializeField]
    public State state;
    int speed = 3;

    //FireObject
    public float BulletSpeed;
    public GameObject enemyBullet;
    private float bulletTime;

    [SerializeField]
    private float bulletTimeEr;

    [SerializeField]
    public Transform SpawnPoint;

    //CoolDown var
    [SerializeField]
    float timerCoolDownAttack = 0;
    bool timerReachedCoolDownAttack = false;

    [SerializeField]
    float timerCoolKnockBack = 0;
    bool timerReachedCoolKnockBack = false;

    //Heath and Canvas
    [SerializeField]
    EnemyHealth health;

    [SerializeField]
    PlayerWeapon playerWeapon;

    void Start()
    {
        playerWeapon = GameObject.Find("PlayerSwordHitbox").GetComponent<PlayerWeapon>();
        state = State.Ready;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        health = GetComponent<EnemyHealth>();
    }

    /*
    void Update(){
        CheckHealth();
        if(state != State.Dead){
            CoolDownAttaickTime();
            CooldownKnockBackTime();
            playerInsight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
            PlayerInAttackrange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
            PlayerInRetreatRange = Physics.CheckSphere(transform.position, RetreatRange, playerLayer);
            if (!playerInsight && !PlayerInAttackrange && state == State.Ready)Patrol();
            if (playerInsight && !PlayerInAttackrange && !PlayerInRetreatRange && state == State.Ready)Chase();
            if (playerInsight && PlayerInAttackrange && state == State.Ready)Shoot();
            if (playerInsight && PlayerInAttackrange && PlayerInRetreatRange && state == State.Cooldown)Retreat();
        }
    }
    */

    void Update()
    {
        CheckHealth();

        if (state != State.Dead)
        {
            // Update cooldowns
            CoolDownAttaickTime();
            CooldownKnockBackTime();

            // Check player distances
            playerInsight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
            PlayerInAttackrange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
            PlayerInRetreatRange = Physics.CheckSphere(
                transform.position,
                RetreatRange,
                playerLayer
            );

            // Determine behavior based on state and player distance
            switch (state)
            {
                case State.Ready:
                    if (playerInsight)
                    {
                        if (PlayerInAttackrange)
                        {
                            Shoot();
                        }
                        else
                        {
                            Chase();
                        }
                    }
                    break;

                case State.Cooldown:
                    if (PlayerInRetreatRange)
                    {
                        Retreat();
                    }
                    else
                    {
                        agent.SetDestination(transform.position);
                    }
                    break;
                case State.Dead:
                    break;
            }
        }
    }

    void CheckHealth()
    {
        if (health.GetCurrentHealth() <= 0 && state != State.Dead)
        {
            Dead();
        }
        else { }
    }

    private void CooldownKnockBackTime()
    {
        if (!timerReachedCoolKnockBack && state == State.KnockBack)
            timerCoolKnockBack += Time.deltaTime;
        if (!timerReachedCoolKnockBack && timerCoolKnockBack > 5 && state == State.KnockBack)
        {
            agent.speed = speed;
            state = State.Ready;
            timerCoolKnockBack = 0;
        }
    }

    void CoolDownAttaickTime()
    {
        if (!timerReachedCoolDownAttack && state == State.Cooldown)
            timerCoolDownAttack += Time.deltaTime;
        if (!timerReachedCoolDownAttack && timerCoolDownAttack > 5 && state == State.Cooldown)
        {
            state = State.Ready;
            timerCoolDownAttack = 0;
        }
    }

    void KnockBack()
    {
        state = State.KnockBack;
        agent.transform.LookAt(player.transform);
    }

    void Retreat()
    {
        agent.SetDestination(-player.transform.position);
    }

    void Chase()
    {
        agent.SetDestination(player.transform.position);
    }

    void Shoot()
    {
        // Stop the NavMeshAgent movement
        agent.isStopped = true; // Prevents the agent from moving during shooting
        agent.velocity = Vector3.zero;

        // Look at the player to aim
        agent.transform.LookAt(player.transform);

        // Transition to cooldown state after shooting
        state = State.Cooldown;

        // Fire bullets
        int numberOfBullets = 3; // Number of bullets fired in one attack
        float spreadAngle = 15f; // Angle in degrees for the scatter
        float bulletDelay = 0.2f; // Delay between bullets

        StartCoroutine(ShootWithDelay(numberOfBullets, spreadAngle, bulletDelay));
    }

    private IEnumerator ShootWithDelay(int numberOfBullets, float spreadAngle, float bulletDelay)
    {
        for (int i = 0; i < numberOfBullets; i++)
        {
            // Instantiate the bullet
            GameObject bullet = Instantiate(
                enemyBullet,
                SpawnPoint.transform.position,
                SpawnPoint.transform.rotation
            );

            // Calculate scatter direction
            Vector3 scatterDirection = SpawnPoint.forward;
            scatterDirection =
                Quaternion.Euler(
                    UnityEngine.Random.Range(-spreadAngle, spreadAngle), // Yaw (left-right)
                    UnityEngine.Random.Range(-spreadAngle, spreadAngle), // Pitch (up-down)
                    0 // No roll
                ) * scatterDirection;

            // Add force to the bullet in the scatter direction
            Rigidbody bulletRig = bullet.GetComponent<Rigidbody>();
            bulletRig.AddForce(scatterDirection.normalized * BulletSpeed, ForceMode.Impulse);

            // Destroy bullet after 5 seconds
            Destroy(bullet, 5f);

            // Wait for the specified delay before firing the next bullet
            yield return new WaitForSeconds(bulletDelay);
        }

        // After shooting is done, re-enable movement
        agent.isStopped = false; // Allow the agent to move again
    }

    void Dead()
    {
        agent.enabled = false;
        this.tag = "Untagged";
    }

    void Patrol()
    {
        if (!walkpointSet)
            SearchForDest();
        if (walkpointSet)
            agent.SetDestination(destPoint);
        if (Vector3.Distance(transform.position, destPoint) < 10)
            walkpointSet = false;
    }

    void SearchForDest()
    {
        float z = UnityEngine.Random.Range(-range, range);
        float x = UnityEngine.Random.Range(-range, range);

        destPoint = new Vector3(
            transform.position.x + x,
            transform.position.y,
            transform.position.z + z
        );

        if (Physics.Raycast(destPoint, Vector3.down, groundLayer))
        {
            walkpointSet = true;
        }
    }

    void StartKnockBack()
    {
        agent.speed = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && other.gameObject.CompareTag("PlayerSword"))
        {
            //health.CalculateDamage(playerWeapon.damage);
            KnockBack();
            StartKnockBack();
        }
    }
}
