using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class BulletStraight : MonoBehaviour
{
    [SerializeField]
    private float speed = 15f;

    [SerializeField]
    private float waitTime = 0f;

    private bool canMove = false;

    void Start()
    {
        StartCoroutine(WaitBeforeMoving());
    }

    void Update()
    {
        if (!canMove)
            return;

        float distanceThisFrame = Time.deltaTime * speed;

        transform.Translate(Vector3.forward * distanceThisFrame, Space.Self);

        Destroy(gameObject, 5f);
    }

    private IEnumerator WaitBeforeMoving()
    {
        yield return new WaitForSeconds(waitTime);

        canMove = true;
    }
}
