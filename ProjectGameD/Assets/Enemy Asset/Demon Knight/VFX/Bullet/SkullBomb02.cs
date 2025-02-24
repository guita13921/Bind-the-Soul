using UnityEngine;

public class SkullBomb02 : MonoBehaviour
{
    public float rotationSpeed = 100f; // Speed of rotation
    public int countdownTime = 5; // Time before explosion
    public GameObject explosionEffect; // Assign a particle system for explosion
    
    private float timer;
    private bool isCountingDown = false;

    [Header("Indicator")]
    [SerializeField] BombIndicator attackIndicatorController;

    [SerializeField]Canvas bar;
    [SerializeField]protected EnemyHealth health;


    void Start()
    {
        health = GetComponent<EnemyHealth>();
        ShowIndicator(countdownTime);
        timer = countdownTime;
        isCountingDown = true;
    }

    void Update()
    {
        CheckHealth();
        RotateSkull();
        if (isCountingDown)
        {
            Countdown();
        }
    }
    
    public void CheckHealth()
    {
        if (health.GetCurrentHealth() <= 0)
        {
            Destroy(gameObject);
        }
    }
    

    void RotateSkull()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    void Countdown()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        HideIndicator();
        Destroy(gameObject);
    }

    void ShowIndicator(int AttackTimeFrame)
    {
        if (attackIndicatorController != null)
        {
            //attackIndicatorCanvas.enabled = true;
            attackIndicatorController.ShowIndicator(AttackTimeFrame);
        }
    }

    void HideIndicator()
    {
        if (attackIndicatorController != null)
        {
            attackIndicatorController.HideIndicator();
        }
    }

     public void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && other.gameObject.CompareTag("PlayerSword"))
        {
            PlayerWeapon playerWeapon = other.gameObject.GetComponent<PlayerWeapon>();
            if (playerWeapon != null) health.CalculateDamage(playerWeapon.damage);

        }
    }
}
