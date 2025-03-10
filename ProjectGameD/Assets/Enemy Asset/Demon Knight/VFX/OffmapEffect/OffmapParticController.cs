using UnityEngine;

public class ExpandingRingHitbox : MonoBehaviour
{
    public SphereCollider hitboxCollider;
    public ParticleSystem particleEffect;
    public float growthSpeed; // Speed at which the ring expands
    public float ringThickness; // How thick the damageable ring is
    public float maxRadius = 5f; // Maximum expansion size
    
    [SerializeField] private float currentRadius = 0f;
    [SerializeField] private float distance = 0f;
    [SerializeField] private int damage;
    [SerializeField] private Health player;

    
    PlayerControl playerControl;
    public PlayerCombat playerCombat;
    [SerializeField] public CharacterData characterData;
    private float reducedDamageSecond = 0; // if HP < 25% of maxHP
    

    void Start()
    {
        playerControl = FindObjectOfType<PlayerControl>();
        if (hitboxCollider == null)
            hitboxCollider = GetComponent<SphereCollider>();

        if (particleEffect == null)
            particleEffect = GetComponent<ParticleSystem>();

        if (hitboxCollider == null || particleEffect == null)
        {
            Debug.LogError("Missing required components!");
            enabled = false;
            return;
        }

        hitboxCollider.isTrigger = true; // Ensure it's a trigger
        hitboxCollider.radius = 0f; // Start at zero
    }

    void Update()
    {
        if (particleEffect.isPlaying)
        {
            currentRadius = Mathf.MoveTowards(currentRadius, maxRadius, growthSpeed * Time.deltaTime);
            hitboxCollider.radius = currentRadius;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            distance = Vector3.Distance(this.transform.position, other.transform.position);

            // Player is inside the ring but not the center
            if (distance >= currentRadius*3 - ringThickness)
            //&& distance <= currentRadius)
            {
                //enemyWeapon.enabled = true;
                //Debug.Log("Player hit by expanding ring!");
                ApplyDamage(other);
            }
        }
    }

    private void ApplyDamage(Collider other)
    {
     
        //Debug.Log("ApplyDamage");
        player = other.gameObject.GetComponent<Health>();
        playerCombat = other.gameObject.GetComponent<PlayerCombat>();
        
           if(playerControl != null){
            playerControl.GetHit();
        }
        if (player != null && other.CompareTag("Player"))
        {
            if (!playerCombat.isShield1 && !playerCombat.isShield2)
            {
                if (player.currentHealth < (player.maxHealth * 0.25f))
                {
                    reducedDamageSecond = characterData.reduceIncomeDamageDependOnHP * 0.15f; // 0.15f per level (15%, 30%, 45%)
                }
                float damageReductionPercentage = characterData.reduceIncomeDamage * 0.05f; // 0.05f per level (5%, 10%, 15%)
                float reducedDamage = damage * damageReductionPercentage;
                float reducedDamageDependOnHP = damage * reducedDamageSecond;
                player.currentHealth -= Mathf.Max(
                    0,
                    damage - reducedDamage - reducedDamageDependOnHP
                );
            }
        }
    }
}
