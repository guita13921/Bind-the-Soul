using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxController : MonoBehaviour
{
    public float enabletime = 0f;
    private BoxCollider boxCollider;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= enabletime)
        {
            boxCollider.enabled = true;
        }
    }
}
