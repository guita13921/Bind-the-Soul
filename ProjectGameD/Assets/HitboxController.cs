using UnityEngine;

public class HitboxController : MonoBehaviour
{
    [Header("Timing Settings")]
    public float enabletime = 0f;
    public bool loop = false;

    [Tooltip("Interval between collider toggles when looping.")]
    public float looptime = 0f;

    private BoxCollider boxCollider;
    private float timer = 0f;
    private float loopTimer = 0f;
    private bool isColliderEnabled = false;
    public float destroyTime = 0f;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
    }

    void Update()
    {
        if (loop)
        {
            HandleLoop();
        }
        else
        {
            HandleSingleActivation();
        }
    }

    private void HandleLoop()
    {
        timer += Time.deltaTime;
        loopTimer += Time.deltaTime;

        if (timer >= enabletime && !isColliderEnabled)
        {
            ToggleBoxCollider(true);
        }

        if (loopTimer >= looptime)
        {
            ToggleBoxCollider(!boxCollider.enabled);
            loopTimer = 0f;
        }

        if (timer >= destroyTime)
            Destroy(gameObject);
    }

    private void HandleSingleActivation()
    {
        timer += Time.deltaTime;

        if (timer >= enabletime && !isColliderEnabled)
        {
            ToggleBoxCollider(true);
        }

        if (isColliderEnabled && timer >= enabletime + 0.1f)
        {
            Destroy(gameObject);
        }
    }

    private void ToggleBoxCollider(bool state)
    {
        boxCollider.enabled = state;
        isColliderEnabled = state;
    }
}
