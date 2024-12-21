using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiletest : MonoBehaviour
{
    public GameObject[] skills;
    public Transform parentObject;

    void Start()
    {
        StartCoroutine(InstantiateSkillCoroutine());
    }

    // Update is called once per frame
    void Update() { }

    IEnumerator InstantiateSkillCoroutine()
    {
        while (true)
        {
            Instantiate(skills[0], parentObject);
            float randomWaitTime = Random.Range(0.1f, 1.0f);

            yield return new WaitForSeconds(randomWaitTime);
        }
    }
}
